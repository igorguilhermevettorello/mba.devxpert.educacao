using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlataformaEducacional.Core.Enumerators;
using PlataformaEducacional.Core.Messages.Integration;
using PlataformaEducacional.Core.Notifications;
using PlataformaEducacional.Pagamentos.Api.Models;
using PlataformaEducacional.Pagamentos.Api.Models.DTOs;
using PlataformaEducacional.Pagamentos.Api.Models.Enums;
using PlataformaEducacional.Pagamentos.Api.Models.ValueObjects;
using PlataformaEducacional.Pagamentos.Api.Services;
using PlataformaEducacional.WebApi.Core.Controllers.Base;
using PlataformaEducacional.WebApi.Core.User;

namespace PlataformaEducacional.Pagamentos.Api.Controllers
{
    [ApiController]
    [Route("api/pagamentos")]
    [Authorize]
    public class PagamentosController : MainController
    {
        readonly IPagamentoRepository _pagamentoRepository;
        readonly IPagamentoService _pagamentoService;
        private readonly IMapper _mapper;
        private readonly IAspNetUser _user;
        private readonly IAlunoService _alunoService;
        private readonly ILogger<PagamentosController> _logger;

        public PagamentosController(
            IPagamentoRepository pagamentoRepository,
            IMapper mapper,
            IPagamentoService pagamentoService,
            INotificador notificador,
            IAspNetUser user,
            IAlunoService alunoService,
            ILogger<PagamentosController> logger) : base(notificador)
        {
            _pagamentoRepository = pagamentoRepository;
            _mapper = mapper;
            _pagamentoService = pagamentoService;
            _user = user;
            _alunoService = alunoService;
            _logger = logger;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(PagamentoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId([FromRoute] Guid id)
        {
            _logger.LogInformation("Solicitação para obter Pagamento {PagamentoId}", id);

            var pagamento = await _pagamentoRepository.ObterPorId(id);

            if (pagamento == null)
            {
                _logger.LogWarning("Pagamento {PagamentoId} não encontrado", id);
                return NotFound();
            }

            if (!await UsuarioPodeVerPagamento(pagamento))
            {
                _logger.LogWarning("Acesso negado para Pagamento {PagamentoId} pelo usuário {UserId}", id, ObterIdUsuario());
                return Forbid();
            }

            _logger.LogInformation("Pagamento {PagamentoId} obtido com sucesso", id);
            var pagamentoDto = _mapper.Map<PagamentoDto>(pagamento);
            return Ok(pagamentoDto);
        }

        [HttpGet("matricula/{matriculaId:guid}")]
        [ProducesResponseType(typeof(PagamentoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorMatriculaId([FromRoute] Guid matriculaId)
        {
            var pagamento = await _pagamentoRepository.ObterPorMatriculaId(matriculaId);

            if (pagamento == null)
            {
                return NotFound();
            }

            if (!await UsuarioPodeVerPagamento(pagamento))
            {
                return Forbid();
            }

            var pagamentoDto = _mapper.Map<PagamentoDto>(pagamento);
            return Ok(pagamentoDto);
        }

        [HttpPost]
        [ProducesResponseType(typeof(PagamentoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RealizarPagamento([FromBody] RealizarPagamentoDto model)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            _logger.LogInformation("Solicitação para realizar Pagamento - MatriculaId {MatriculaId}, Valor {Valor}", model.MatriculaId, model.ValorCurso);

            if (!await UsuarioPodePagarAMatricula(model.MatriculaId))
            {
                _logger.LogWarning("Acesso negado para pagar Matrícula {MatriculaId} pelo usuário {UserId}", model.MatriculaId, ObterIdUsuario());
                return Forbid();
            }

            var cartaoCredito = CartaoCredito.TryCreate(model.TitularCartao, model.NumeroCartao, model.ValidadeCartao, model.CodigoSegurancaCartao);

            if (!cartaoCredito.IsValid)
            {
                _logger.LogWarning("Cartão de crédito inválido para Matrícula {MatriculaId}: {Errors}", model.MatriculaId,
                    string.Join(", ", cartaoCredito.ValidationResult.Errors.Select(e => e.ErrorMessage)));
                return CustomResponse(cartaoCredito.ValidationResult);
            }

            var pagamento = new Pagamento(
                model.MatriculaId,
                TipoPagamento.CartaoCredito,    //Tipo pagamento chumbado pois o prop�sito da api � apenas did�tico
                model.ValorCurso,
                cartaoCredito.Card!);

            var responseMessage = await _pagamentoService.AutorizarPagamento(pagamento);

            if (!responseMessage.ValidationResult.IsValid)
            {
                _logger.LogWarning("Falha ao autorizar Pagamento {PagamentoId} para Matrícula {MatriculaId}: {Errors}", pagamento.Id, model.MatriculaId,
                    string.Join(", ", responseMessage.ValidationResult.Errors.Select(e => e.ErrorMessage)));
                NotificarErros(responseMessage);
                return CustomResponse();
            }

            _logger.LogInformation("Pagamento autorizado com sucesso - PagamentoId {PagamentoId}, MatriculaId {MatriculaId}, Valor {Valor}",
                pagamento.Id, model.MatriculaId, model.ValorCurso);
            return CreatedAtAction(nameof(ObterPorId), new { id = pagamento.Id }, _mapper.Map<PagamentoDto>(pagamento));
        }

        private void NotificarErros(ResponseMessage responseMessage)
        {
            foreach (var erro in responseMessage.ValidationResult.Errors)
            {
                NotificarErro(erro.PropertyName, erro.ErrorMessage);
            }
        }

        private async Task<bool> UsuarioPodeVerPagamento(Pagamento pagamento)
        {
            var perfil = ObterPerfilUsuario();

            if (perfil == TipoUsuario.Administrador)
                return true;

            var usuarioId = ObterIdUsuario();
            var matricula = await _alunoService.ObterMatriculaPorIdAsync(pagamento.MatriculaId);

            if (usuarioId == null || matricula?.AlunoId.ToString() != usuarioId)
                return false;

            return true;
        }

        private async Task<bool> UsuarioPodePagarAMatricula(Guid matriculaId)
        {
            var perfil = ObterPerfilUsuario();

            if (perfil == TipoUsuario.Administrador)
                return true;

            var usuarioId = ObterIdUsuario();
            var matricula = await _alunoService.ObterMatriculaPorIdAsync(matriculaId);

            if (usuarioId == null || matricula?.AlunoId.ToString() != usuarioId)
                return false;

            return true;
        }
    }
}

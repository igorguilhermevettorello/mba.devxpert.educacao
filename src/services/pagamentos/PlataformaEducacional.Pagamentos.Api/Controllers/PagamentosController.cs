using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public PagamentosController(
            IPagamentoRepository pagamentoRepository,
            IMapper mapper,
            IPagamentoService pagamentoService,
            INotificador notificador,
            IAspNetUser user) : base(notificador)
        {
            _pagamentoRepository = pagamentoRepository;
            _mapper = mapper;
            _pagamentoService = pagamentoService;
            _user = user;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(PagamentoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId([FromRoute] Guid id)
        {
            var pagamento = await _pagamentoRepository.ObterPorId(id);

            if (pagamento == null)
            {
                return NotFound();
            }

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

            var cartaoCredito = CartaoCredito.Criar(model.TitularCartao, model.NumeroCartao, model.ValidadeCartao, model.CodigoSegurancaCartao);

            var pagamento = new Pagamento(
                model.MatriculaId,
                TipoPagamento.CartaoCredito,    //Tipo pagamento chumbado pois o propósito da api é apenas didático
                model.ValorCurso,
                cartaoCredito);

            var responseMessage = await _pagamentoService.AutorizarPagamento(pagamento);

            if (!responseMessage.ValidationResult.IsValid)
            {
                NotificarErros(responseMessage);
                return CustomResponse();
            }

            return CreatedAtAction(nameof(ObterPorId), new { id = pagamento.Id }, _mapper.Map<PagamentoDto>(pagamento));
        }

        private void NotificarErros(ResponseMessage responseMessage)
        {
            foreach (var erro in responseMessage.ValidationResult.Errors)
            {
                NotificarErro(erro.PropertyName, erro.ErrorMessage);
            }
        }
    }
}

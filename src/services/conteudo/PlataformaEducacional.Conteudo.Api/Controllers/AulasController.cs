using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlataformaEducacional.Conteudo.Api.DTOs;
using PlataformaEducacional.Conteudo.Api.DTOs.Aulas;
using PlataformaEducacional.Conteudo.Application.Commands.Aulas;
using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Core.Mediator;
using PlataformaEducacional.Core.Notifications;
using PlataformaEducacional.WebApi.Core.Controllers.Base;

namespace PlataformaEducacional.Conteudo.Api.Controllers
{
    [ApiController]
    [Route("api/aulas")]
    [Authorize]
    public class AulasController : MainController
    {
        private readonly IMediatorHandler _mediatorHandler;
        private readonly IAulaRepository _aulaRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AulasController> _logger;

        public AulasController(
            IMediatorHandler mediatorHandler,
            IMapper mapper,
            IAulaRepository aulaRepository,
            INotificador notificador,
            ILogger<AulasController> logger) : base(notificador)
        {
            _aulaRepository = aulaRepository;
            _mediatorHandler = mediatorHandler;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(ResultDto<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> Criar([FromBody] CriarAulaDto criarAulaDto)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            _logger.LogInformation("Solicitação para criar nova Aula no Curso {CursoId}: {Titulo}", criarAulaDto.CursoId, criarAulaDto.Titulo);

            var command = new CriarAulaCommand(
                criarAulaDto.CursoId,
                criarAulaDto.Titulo,
                criarAulaDto.Descricao,
                criarAulaDto.DuracaoMinutos,
                criarAulaDto.Ordem
            );

            var resultado = await _mediatorHandler.SendCommand(command);

            if (!resultado.IsValid)
            {
                _logger.LogWarning("Falha ao criar Aula no Curso {CursoId}: {Errors}", criarAulaDto.CursoId,
                    string.Join(", ", resultado.Errors.Select(e => e.ErrorMessage)));
                foreach (var erro in resultado.Errors)
                    NotificarErro(erro.PropertyName, erro.ErrorMessage);
                return CustomResponse();
            }

            _logger.LogInformation("Aula criada com sucesso - AulaId {AulaId}, CursoId {CursoId}, Titulo {Titulo}",
                command.AggregateId, criarAulaDto.CursoId, criarAulaDto.Titulo);
            var response = ResultDto.Ok(command.AggregateId, "Aula criada com sucesso");
            return CreatedAtAction(nameof(ObterPorId), new { id = command.AggregateId }, response);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(ResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Atualizar(Guid id, [FromBody] AtualizarAulaDto atualizarAulaDto)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            _logger.LogInformation("Solicitação para atualizar Aula {AulaId}: {Titulo}", id, atualizarAulaDto.Titulo);

            var command = new AtualizarAulaCommand
            {
                Id = id,
                Titulo = atualizarAulaDto.Titulo,
                Descricao = atualizarAulaDto.Descricao,
                DuracaoMinutos = atualizarAulaDto.DuracaoMinutos,
                Ordem = atualizarAulaDto.Ordem
            };

            var resultado = await _mediatorHandler.SendCommand(command);

            if (!resultado.IsValid)
            {
                _logger.LogWarning("Falha ao atualizar Aula {AulaId}: {Errors}", id,
                    string.Join(", ", resultado.Errors.Select(e => e.ErrorMessage)));
                return CustomResponse();
            }

            _logger.LogInformation("Aula atualizada com sucesso - AulaId {AulaId}", id);
            var response = ResultDto.Ok("Aula atualizada com sucesso");
            return CustomResponse(response);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResultDto<AulaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ObterPorId(Guid id)
        {
            var aula = await _aulaRepository.BuscarPorIdAsync(id);
            if (aula == null)
            {
                NotificarErro("Aula", "Aula não encontrada");
                return NotFound();
            }

            var aulaDto = _mapper.Map<AulaDto>(aula);
            var response = ResultDto.Ok(aulaDto, "Aula obtida com sucesso");
            return CustomResponse(response);
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResultDto<IEnumerable<AulaDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> Listar(
            [FromQuery] Guid? cursoId = null,
            [FromQuery] bool apenasAtivas = false)
        {
            var command = new ListarAulasCommand(cursoId, apenasAtivas);
            var aulas = await _mediatorHandler.SendQuery<IEnumerable<Aula>>(command);
            var aulasDto = _mapper.Map<IEnumerable<AulaDto>>(aulas);
            var response = ResultDto.Ok(aulasDto, "Aulas obtidas com sucesso");
            return CustomResponse(response);
        }

        [HttpGet("curso/{cursoId:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResultDto<IEnumerable<AulaDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> ListarPorCurso(Guid cursoId, [FromQuery] bool apenasAtivas = false)
        {
            IEnumerable<Aula> aulas;

            if (apenasAtivas)
                aulas = await _aulaRepository.ObterAtivasPorCursoIdAsync(cursoId);
            else
                aulas = await _aulaRepository.ObterPorCursoIdAsync(cursoId);

            var aulasDto = _mapper.Map<IEnumerable<AulaDto>>(aulas);
            var response = ResultDto.Ok(aulasDto, $"Aulas do curso obtidas com sucesso");
            return CustomResponse(response);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(ResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Deletar(Guid id)
        {
            _logger.LogInformation("Solicitação para deletar Aula {AulaId}", id);

            var command = new DeletarAulaCommand(id);
            var resultado = await _mediatorHandler.SendCommand(command);

            if (!resultado.IsValid)
            {
                _logger.LogWarning("Falha ao deletar Aula {AulaId}: {Errors}", id,
                    string.Join(", ", resultado.Errors.Select(e => e.ErrorMessage)));
                return CustomResponse();
            }

            _logger.LogInformation("Aula deletada com sucesso - AulaId {AulaId}", id);
            var response = ResultDto.Ok("Aula deletada com sucesso");
            return CustomResponse(response);
        }
    }
}

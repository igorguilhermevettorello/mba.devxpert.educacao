using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacional.Conteudo.Api.DTOs;
using PlataformaEducacional.Conteudo.Api.DTOs.Aulas;
using PlataformaEducacional.Conteudo.Application.Commands.Aulas;
using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Core.Mediator;
using PlataformaEducacional.Core.Notifications;
using PlataformaEducacional.WebApi.Core.Controllers.Base;

namespace PlataformaEducacional.Conteudo.Api.Controllers
{
    //[Authorize(Roles = nameof(TipoUsuario.Administrador))]
    [ApiController]
    [Route("api/aulas")]
    public class AulasController : MainController
    {
        private readonly IMediatorHandler _mediatorHandler;
        private readonly IMapper _mapper;

        public AulasController(IMediatorHandler mediatorHandler, IMapper mapper, INotificador notificador)
            : base(notificador)
        {
            _mediatorHandler = mediatorHandler;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultDto<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> Criar([FromBody] CriarAulaDto criarAulaDto)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

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
                foreach (var erro in resultado.Errors)
                    NotificarErro(erro.PropertyName, erro.ErrorMessage);
                return CustomResponse();
            }

            var response = ResultDto.Ok(command.AggregateId, "Aula criada com sucesso");
            return CreatedAtAction(nameof(ObterPorId), new { id = command.AggregateId }, response);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Atualizar(Guid id, [FromBody] AtualizarAulaDto atualizarAulaDto)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

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
                foreach (var erro in resultado.Errors)
                    NotificarErro(erro.PropertyName, erro.ErrorMessage);
                return CustomResponse();
            }

            var response = ResultDto.Ok("Aula atualizada com sucesso");
            return CustomResponse(response);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ResultDto<AulaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ObterPorId(Guid id)
        {
            var command = new ObterAulaPorIdCommand(id);
            var aula = await _mediatorHandler.SendQuery<Aula?>(command);

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
        [ProducesResponseType(typeof(ResultDto<IEnumerable<AulaDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> Listar([FromQuery] Guid? cursoId = null, [FromQuery] bool apenasAtivas = false)
        {
            var command = new ListarAulasCommand(cursoId, apenasAtivas);
            var aulas = await _mediatorHandler.SendQuery<IEnumerable<Aula>>(command);
            var aulasDto = _mapper.Map<IEnumerable<AulaDto>>(aulas);
            var response = ResultDto.Ok(aulasDto, "Aulas obtidas com sucesso");
            return CustomResponse(response);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Deletar(Guid id)
        {
            var command = new DeletarAulaCommand(id);
            var resultado = await _mediatorHandler.SendCommand(command);

            if (!resultado.IsValid)
            {
                foreach (var erro in resultado.Errors)
                    NotificarErro(erro.PropertyName, erro.ErrorMessage);
                return CustomResponse();
            }

            var response = ResultDto.Ok("Aula deletada com sucesso");
            return CustomResponse(response);
        }

        [HttpPut("{id:guid}/ativar")]
        [ProducesResponseType(typeof(ResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Ativar(Guid id)
        {
            var command = new AtivarAulaCommand { AulaId = id };
            var resultado = await _mediatorHandler.SendCommand(command);

            if (!resultado.IsValid)
            {
                foreach (var erro in resultado.Errors)
                    NotificarErro(erro.PropertyName, erro.ErrorMessage);
                return CustomResponse();
            }

            var response = ResultDto.Ok("Aula ativada com sucesso");
            return CustomResponse(response);
        }

        [HttpPut("{id:guid}/inativar")]
        [ProducesResponseType(typeof(ResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Inativar(Guid id)
        {
            var command = new InativarAulaCommand(id);
            var resultado = await _mediatorHandler.SendCommand(command);

            if (!resultado.IsValid)
            {
                foreach (var erro in resultado.Errors)
                    NotificarErro(erro.PropertyName, erro.ErrorMessage);
                return CustomResponse();
            }

            var response = ResultDto.Ok("Aula inativada com sucesso");
            return CustomResponse(response);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacional.Conteudo.Api.DTOs;
using PlataformaEducacional.Conteudo.Api.DTOs.Cursos;
using PlataformaEducacional.Conteudo.Application.Commands.Cursos;
using PlataformaEducacional.Core.Mediator;
using PlataformaEducacional.Core.Notifications;
using PlataformaEducacional.WebApi.Core.Controllers.Base;

namespace PlataformaEducacional.Conteudo.Api.Controllers
{
    [ApiController]
    [Route("api/cursos")]
    public class CursosController : MainController
    {
        private readonly IMediatorHandler _mediatorHandler;
        //private readonly IMapper _mapper;

        public CursosController(IMediatorHandler mediatorHandler, INotificador notificador)
            : base(notificador)
        {
            _mediatorHandler = mediatorHandler;
            //_mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultDto<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> Criar([FromBody] CriarCursoDto criarCursoDto)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            ConteudoProgramaticoCommand? conteudoProgramaticoCommand = null;

            if (criarCursoDto.ConteudoProgramatico != null)
            {
                conteudoProgramaticoCommand = new ConteudoProgramaticoCommand
                {
                    Ementa = criarCursoDto.ConteudoProgramatico.Ementa,
                    Objetivo = criarCursoDto.ConteudoProgramatico.Objetivo,
                    Bibliografia = criarCursoDto.ConteudoProgramatico.Bibliografia,
                    MaterialUrl = criarCursoDto.ConteudoProgramatico.MaterialUrl
                };
            }

            var command = new CriarCursoCommand(
                criarCursoDto.Titulo,
                criarCursoDto.Descricao,
                criarCursoDto.Instrutor,
                criarCursoDto.Nivel,
                criarCursoDto.Valor,
                conteudoProgramaticoCommand
            );

            var resultado = await _mediatorHandler.SendCommand(command);

            if (!resultado.IsValid)
            {
                foreach (var erro in resultado.Errors)
                    NotificarErro(erro.PropertyName, erro.ErrorMessage);
                return CustomResponse();
            }

            var response = ResultDto.Ok(command.AggregateId, "Curso criado com sucesso");
            return CreatedAtAction(nameof(ObterPorId), new { id = command.AggregateId }, response);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Atualizar(Guid id, [FromBody] AtualizarCursoDto atualizarCursoDto)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            ConteudoProgramaticoCommand? conteudoProgramaticoCommand = null;

            if (atualizarCursoDto.ConteudoProgramatico != null)
            {
                conteudoProgramaticoCommand = new ConteudoProgramaticoCommand
                {
                    Ementa = atualizarCursoDto.ConteudoProgramatico.Ementa,
                    Objetivo = atualizarCursoDto.ConteudoProgramatico.Objetivo,
                    Bibliografia = atualizarCursoDto.ConteudoProgramatico.Bibliografia,
                    MaterialUrl = atualizarCursoDto.ConteudoProgramatico.MaterialUrl
                };
            }

            var command = new AtualizarCursoCommand
            {
                Id = id,
                Titulo = atualizarCursoDto.Titulo,
                Descricao = atualizarCursoDto.Descricao,
                Nivel = atualizarCursoDto.Nivel,
                ConteudoProgramatico = conteudoProgramaticoCommand
            };

            var resultado = await _mediatorHandler.SendCommand(command);

            //if (!resultado)
            //    return CustomResponse();

            var response = ResultDto.Ok("Curso atualizado com sucesso");
            return CustomResponse(response);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ResultDto<CursoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ObterPorId(Guid id)
        {
            var command = new ObterCursoPorIdCommand(id);
            //var curso = await _mediatorHandler.SendCommand(command);
            //if (curso == null)
            //{
            //    NotificarErro("Curso", "Curso não encontrado");
            //    return NotFound();
            //}
            //var cursoDto = _mapper.Map<CursoDto>(curso);
            //var response = ResultDto.Ok(cursoDto, "Curso obtido com sucesso");
            //return CustomResponse(response);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("ativos")]
        [ProducesResponseType(typeof(ResultDto<IEnumerable<CursoDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> ListarCursosAtivos()
        {
            var command = new ListarCursosCommand(apenasAtivos: true);
            //var cursos = await _mediatorHandler.SendCommand(command);
            //var cursosDto = _mapper.Map<IEnumerable<CursoDto>>(cursos);
            //var response = ResultDto.Ok(cursosDto, "Cursos ativos obtidos com sucesso");
            //return CustomResponse(response);
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultDto<IEnumerable<CursoDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> Listar([FromQuery] bool apenasAtivos = false)
        {
            var command = new ListarCursosCommand(apenasAtivos);
            //var cursos = await _mediatorHandler.SendCommand(command);
            //var cursosDto = _mapper.Map<IEnumerable<CursoDto>>(cursos);
            //var response = ResultDto.Ok(cursosDto, "Cursos obtidos com sucesso");
            //return CustomResponse(response);
            return Ok();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Deletar(Guid id)
        {
            var command = new DeletarCursoCommand(id);
            var resultado = await _mediatorHandler.SendCommand(command);

            //if (!resultado)
            //    return CustomResponse();

            var response = ResultDto.Ok("Curso deletado com sucesso");
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
            var command = new AtivarCursoCommand { CursoId = id };
            var resultado = await _mediatorHandler.SendCommand(command);

            //if (!resultado)
            //    return CustomResponse();

            var response = ResultDto.Ok("Curso ativado com sucesso");
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
            var command = new InativarCursoCommand(id);
            var resultado = await _mediatorHandler.SendCommand(command);

            //if (!resultado)
            //    return CustomResponse();

            var response = ResultDto.Ok("Curso inativado com sucesso");
            return CustomResponse(response);
        }
    }
}

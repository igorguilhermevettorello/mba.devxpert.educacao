using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlataformaEducacional.Conteudo.Api.DTOs;
using PlataformaEducacional.Conteudo.Api.DTOs.Cursos;
using PlataformaEducacional.Conteudo.Application.Commands.Cursos;
using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Core.Mediator;
using PlataformaEducacional.Core.Notifications;
using PlataformaEducacional.WebApi.Core.Controllers.Base;

namespace PlataformaEducacional.Conteudo.Api.Controllers;

[ApiController]
[Route("api/cursos")]
[Authorize]
public class CursosController : MainController
{
    private readonly IMediatorHandler _mediatorHandler;
    private readonly ICursoRepository _cursoRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CursosController> _logger;

    public CursosController(
        IMediatorHandler mediatorHandler,
        IMapper mapper,
        ICursoRepository cursoRepository,
        INotificador notificador,
        ILogger<CursosController> logger) : base(notificador)
    {
        _cursoRepository = cursoRepository;
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
    public async Task<ActionResult> Criar([FromBody] CriarCursoDto criarCursoDto)
    {
        if (!ModelState.IsValid)
            return CustomResponse(ModelState);

        _logger.LogInformation("Solicitação para criar novo Curso: {Titulo}", criarCursoDto.Titulo);

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
            _logger.LogWarning("Falha ao criar Curso {Titulo}: {Errors}", criarCursoDto.Titulo,
                string.Join(", ", resultado.Errors.Select(e => e.ErrorMessage)));
            foreach (var erro in resultado.Errors)
                NotificarErro(erro.PropertyName, erro.ErrorMessage);
            return CustomResponse();
        }

        _logger.LogInformation("Curso criado com sucesso - CursoId {CursoId}, Titulo {Titulo}", command.AggregateId, criarCursoDto.Titulo);
        var response = ResultDto.Ok(command.AggregateId, "Curso criado com sucesso");
        return CreatedAtAction(nameof(ObterPorId), new { id = command.AggregateId }, response);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(ResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Atualizar(Guid id, [FromBody] AtualizarCursoDto atualizarCursoDto)
    {
        if (!ModelState.IsValid)
            return CustomResponse(ModelState);

        _logger.LogInformation("Solicitação para atualizar Curso {CursoId}: {Titulo}", id, atualizarCursoDto.Titulo);

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
            ConteudoProgramatico = conteudoProgramaticoCommand,
            Instrutor = atualizarCursoDto.Instrutor,
            Valor = atualizarCursoDto.Valor
        };

        var resultado = await _mediatorHandler.SendCommand(command);

        if (!resultado.IsValid)
        {
            _logger.LogWarning("Falha ao atualizar Curso {CursoId}: {Errors}", id,
                string.Join(", ", resultado.Errors.Select(e => e.ErrorMessage)));
            return CustomResponse();
        }

        _logger.LogInformation("Curso atualizado com sucesso - CursoId {CursoId}", id);
        var response = ResultDto.Ok("Curso atualizado com sucesso");
        return CustomResponse(response);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResultDto<CursoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ObterPorId(Guid id)
    {
        var curso = await _cursoRepository.BuscarPorIdAsync(id);
        if (curso == null)
        {
            NotificarErro("Curso", "Curso não encontrado");
            return NotFound();
        }

        var cursoDto = _mapper.Map<CursoDto>(curso);
        var response = ResultDto.Ok(cursoDto, "Curso obtido com sucesso");
        return CustomResponse(response);
    }

    [HttpGet("ativos")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResultDto<IEnumerable<CursoDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> ListarCursosAtivos()
    {
        var cursos = await _cursoRepository.ObterAtivosAsync();
        var cursosDto = _mapper.Map<IEnumerable<CursoDto>>(cursos);
        var response = ResultDto.Ok(cursosDto, "Cursos ativos obtidos com sucesso");
        return CustomResponse(response);
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(ResultDto<IEnumerable<CursoDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> Listar([FromQuery] bool apenasAtivos = false)
    {
        var command = new ListarCursosCommand(apenasAtivos);
        var cursos = await _mediatorHandler.SendQuery<IEnumerable<Curso>>(command);
        var cursosDto = _mapper.Map<IEnumerable<CursoDto>>(cursos);
        var response = ResultDto.Ok(cursosDto, "Cursos obtidos com sucesso");
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
        _logger.LogInformation("Solicitação para deletar Curso {CursoId}", id);

        var command = new DeletarCursoCommand(id);
        var resultado = await _mediatorHandler.SendCommand(command);

        if (!resultado.IsValid)
        {
            _logger.LogWarning("Falha ao deletar Curso {CursoId}: {Errors}", id,
                string.Join(", ", resultado.Errors.Select(e => e.ErrorMessage)));
            return CustomResponse();
        }

        _logger.LogInformation("Curso deletado com sucesso - CursoId {CursoId}", id);
        var response = ResultDto.Ok("Curso deletado com sucesso");
        return CustomResponse(response);
    }

    [HttpPut("{id:guid}/ativar")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(ResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Ativar(Guid id)
    {
        _logger.LogInformation("Solicitação para ativar Curso {CursoId}", id);

        var command = new AtivarCursoCommand { CursoId = id };
        var resultado = await _mediatorHandler.SendCommand(command);

        if (!resultado.IsValid)
        {
            _logger.LogWarning("Falha ao ativar Curso {CursoId}: {Errors}", id,
                string.Join(", ", resultado.Errors.Select(e => e.ErrorMessage)));
            return CustomResponse();
        }

        _logger.LogInformation("Curso ativado com sucesso - CursoId {CursoId}", id);
        var response = ResultDto.Ok("Curso ativado com sucesso");
        return CustomResponse(response);
    }

    [HttpPut("{id:guid}/inativar")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(ResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Inativar(Guid id)
    {
        _logger.LogInformation("Solicitação para inativar Curso {CursoId}", id);

        var command = new InativarCursoCommand(id);
        var resultado = await _mediatorHandler.SendCommand(command);

        if (!resultado.IsValid)
        {
            _logger.LogWarning("Falha ao inativar Curso {CursoId}: {Errors}", id,
                string.Join(", ", resultado.Errors.Select(e => e.ErrorMessage)));
            return CustomResponse();
        }

        _logger.LogInformation("Curso inativado com sucesso - CursoId {CursoId}", id);
        var response = ResultDto.Ok("Curso inativado com sucesso");
        return CustomResponse(response);
    }
}

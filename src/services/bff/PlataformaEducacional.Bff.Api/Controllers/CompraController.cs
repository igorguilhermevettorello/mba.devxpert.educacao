using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacional.Bff.Api.Interfaces;
using PlataformaEducacional.Bff.Api.Models;
using PlataformaEducacional.WebApi.Core.Controllers;
using PlataformaEducacional.WebApi.Core.User;

namespace PlataformaEducacional.Bff.Api.Controllers;

[Authorize]
public class CompraController : MainController
{
    private readonly IMediator _mediator;
    private readonly IAspNetUser _user;

    private readonly IConteudoService _conteudoService;
    private readonly IMatriculaService _matriculaService;
    private readonly IPagamentoService _pagamentoService;

    public CompraController(IMediator mediator,
        IAspNetUser aspNetUser,
        IConteudoService conteudoService,
        IMatriculaService matriculaService,
        IPagamentoService pagamentoService)
    {
        _mediator = mediator;
        _user = aspNetUser;
        _conteudoService = conteudoService;
        _matriculaService = matriculaService;
        _pagamentoService = pagamentoService;
    }

    //Listar Conteúdos disponíveis para compra
    [HttpGet("cursos-disponiveis")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<IActionResult> ListarConteudosDisponiveis()
    {
        var conteudos = await _conteudoService.ObterCursoDisponiveisAsync();
        return CustomResponse(conteudos);
    }

    //listar matrículas pendentes do aluno
    [HttpGet("alunos/{alunoId}/matriculas/pendentes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListarMatriculaPendente(Guid alunoId)
    {
        var matriculasPendentes = await _matriculaService.ObterMatriculaPendentesAsync(alunoId);
        return CustomResponse(matriculasPendentes);
    }

    [HttpPost("alunos/{alunoId:guid}/matricula")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RealizarMatricula([FromRoute] Guid alunoId, [FromBody] RealizarMatriculaDto model)
    {
        var result = await _matriculaService.RealizarMatriculaAsync(alunoId, model);
        return CustomResponse(result);
    }


    [HttpPost("matricula/{id}/pagamento")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RealizarPagamento(Guid id, [FromBody] RealizarPagamentoDto model)
    {
        if (model.MatriculaId == Guid.Empty) return CustomResponse("MatriculaId é obrigatório.");
        if (model.MatriculaId != id) return CustomResponse("Matricula invalida.");
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var result = await _pagamentoService.RealizarPagamentoAsync(model);

        return CustomResponse(result);
    }
}

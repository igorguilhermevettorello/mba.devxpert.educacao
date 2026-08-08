using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<CompraController> _logger;

    public CompraController(IMediator mediator,
        IAspNetUser aspNetUser,
        IConteudoService conteudoService,
        IMatriculaService matriculaService,
        IPagamentoService pagamentoService,
        ILogger<CompraController> logger)
    {
        _mediator = mediator;
        _user = aspNetUser;
        _conteudoService = conteudoService;
        _matriculaService = matriculaService;
        _pagamentoService = pagamentoService;
        _logger = logger;
    }

    //Listar Conteúdos disponíveis para compra
    [HttpGet("cursos-disponiveis")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<IActionResult> ListarConteudosDisponiveis()
    {
        _logger.LogInformation("Solicitação para listar cursos disponíveis");

        var conteudos = await _conteudoService.ObterCursoDisponiveisAsync();

        if (conteudos?.Data == null || !conteudos.Data.Any())
            _logger.LogWarning("Nenhum curso disponível encontrado");
        else
            _logger.LogInformation("Cursos disponíveis obtidos com sucesso - Total: {Total}", conteudos.Data.Count());

        return CustomResponse(conteudos);
    }

    //listar matrículas pendentes do aluno
    [HttpGet("alunos/{alunoId}/matriculas/pendentes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListarMatriculaPendente(Guid alunoId)
    {
        _logger.LogInformation("Solicitação para listar matrículas pendentes - AlunoId {AlunoId}", alunoId);

        var matriculasPendentes = await _matriculaService.ObterMatriculaPendentesAsync(alunoId);

        if (matriculasPendentes == null || !matriculasPendentes.Any())
            _logger.LogWarning("Nenhuma matrícula pendente encontrada para AlunoId {AlunoId}", alunoId);
        else
            _logger.LogInformation("Matrículas pendentes obtidas - AlunoId {AlunoId}, Total: {Total}", alunoId, matriculasPendentes.Count());

        return CustomResponse(matriculasPendentes);
    }

    [HttpPost("alunos/{alunoId:guid}/matricula")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RealizarMatricula([FromRoute] Guid alunoId, [FromBody] RealizarMatriculaDto model)
    {
        _logger.LogInformation("Solicitação para realizar matrícula - AlunoId {AlunoId}, CursoId {CursoId}", alunoId, model.CursoId);

        var result = await _matriculaService.RealizarMatriculaAsync(alunoId, model);

        if (result != null)
            _logger.LogInformation("Matrícula realizada com sucesso - AlunoId {AlunoId}, CursoId {CursoId}", alunoId, model.CursoId);
        else
            _logger.LogWarning("Falha ao realizar matrícula - AlunoId {AlunoId}", alunoId);

        return CustomResponse(result);
    }


    [HttpPost("matricula/{id}/pagamento")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RealizarPagamento(Guid id, [FromBody] RealizarPagamentoDto model)
    {
        _logger.LogInformation("Solicitação para realizar pagamento - MatriculaId {MatriculaId}, Valor {Valor}", id, model.ValorCurso);

        if (model.MatriculaId == Guid.Empty)
        {
            _logger.LogWarning("MatriculaId vazio na solicitação de pagamento");
            return CustomResponse("MatriculaId é obrigatório.");
        }

        if (model.MatriculaId != id)
        {
            _logger.LogWarning("MatriculaId inconsistente - Rota: {RotaId}, Body: {BodyId}", id, model.MatriculaId);
            return CustomResponse("Matricula invalida.");
        }

        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var result = await _pagamentoService.RealizarPagamentoAsync(model);

        if (result)
            _logger.LogInformation("Pagamento realizado com sucesso - MatriculaId {MatriculaId}, Valor {Valor}", id, model.ValorCurso);
        else
            _logger.LogWarning("Falha ao realizar pagamento - MatriculaId {MatriculaId}", id);

        return CustomResponse(result);
    }
}

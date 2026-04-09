using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacional.Bff.Api.Interfaces;
using PlataformaEducacional.Bff.Api.Models;
using PlataformaEducacional.WebApi.Core.Controllers;
using PlataformaEducacional.WebApi.Core.User;

namespace PlataformaEducacional.Bff.Api.Controllers;

public class CompraController : MainController
{
    private readonly IMediator _mediator;
    private readonly IAspNetUser _user;
    
    private readonly IConteudoService _conteudoService;
    private readonly IMatriculaService _matriculaService;

    public CompraController(IMediator mediator, 
        IAspNetUser aspNetUser,
        IConteudoService conteudoService,
        IMatriculaService matriculaService)
    {
        _mediator = mediator;
        _user = aspNetUser;
        _conteudoService = conteudoService;
        _matriculaService = matriculaService;
    }

    //Listar Conteúdos disponíveis para compra
    [HttpPost("cursos-disponiveis")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListarConteudosDisponiveis()
    {        
        var conteudos = await _conteudoService.ObterCursoDisponiveisAsync();
        return CustomResponse(conteudos);
    }

    //listar matrículas pendentes do aluno
    [HttpPost("matricula-pendente")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListarMatriculaPendente()
    {
        var matriculasPendentes = await _matriculaService.ObterMatriculaPendentesAsync();
        return CustomResponse(matriculasPendentes);
    }

    [HttpPost("matricula")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RealizarMatricula([FromBody] RealizarMatriculaDto model)
    {
        var result = await _matriculaService.RealizarMatriculaAsync(model);
        return CustomResponse(result);
    }


    [HttpPost("matricula/{id}/pagamento")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RealizarPagamento(Guid id)
    {
        // Implementar lógica de registro de progresso utilizando _mediator e _user
        return CustomResponse();
    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacional.WebApi.Core.Controllers;
using PlataformaEducacional.WebApi.Core.User;

namespace PlataformaEducacional.Bff.Api.Controllers;

public class CompraController : MainController
{
    private readonly IMediator _mediator;
    private readonly IAspNetUser _user;

    public CompraController(IMediator mediator, IAspNetUser aspNetUser)
    {
        _mediator = mediator;
        _user = aspNetUser;
    }

    //Listar Conteúdos disponíveis para compra

    //listar matrículas pendentes do aluno

    [HttpPost("matricula")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RealizarMatricula()
    {
        // Implementar lógica de compra utilizando _mediator e _user
        return CustomResponse();
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

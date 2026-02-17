using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacional.Alunos.Api.Application.Commands;
using PlataformaEducacional.Alunos.Api.Models;
using PlataformaEducacional.WebApi.Core.Identity;
using PlataformaEducacional.WebApi.Core.User;

namespace PlataformaEducacional.Alunos.Api.Controllers;

[Authorize]
public class AlunosController : MainController
{
    private readonly IAlunoRepository _alunosRepository;
    private readonly IMediator _mediator;
    private readonly IAspNetUser _user;

    public AlunosController(IAlunoRepository alunosRepository, IMediator mediator, IAspNetUser user)
    {
        _alunosRepository = alunosRepository;
        _mediator = mediator;
        _user = user;
    }

    [HttpGet("endereco")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterEndereco()
    {
        if (_user == null)
            return NotFound();

        var address = await _alunosRepository.ObterEnderecoPorAlunoId(_user.ObterUserId());

        if (address is null)
            return NotFound();

        return CustomResponse(address);
    }

    [HttpPost("endereco")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AdicionarEndereco(AdicionarEnderecoCommand endereco)
    {
        endereco.AlunoId = _user.ObterUserId(); 
        return CustomResponse(await _mediator.Send(endereco));
    }
}

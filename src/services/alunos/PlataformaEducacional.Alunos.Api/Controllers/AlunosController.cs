using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacional.Alunos.Application.Commands;
using PlataformaEducacional.Alunos.Domain.Interfaces;
using PlataformaEducacional.WebApi.Core.Controllers;
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

    [HttpPost("matricula")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RealizarMatricula([FromBody] RealizarMatriculaCommand matriculaCommand)
    {
        var command = new RealizarMatriculaCommand(_user.ObterUserId(), matriculaCommand.CursoId, matriculaCommand.Valor);

        return CustomResponse(await _mediator.Send(command));
    }

    [HttpPost("progresso")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegistrarProgresso([FromBody] RegistrarProgressoCommand progressoCommand)
    {
        var command = new RegistrarProgressoCommand(_user.ObterUserId(), progressoCommand.AulaId);

        return CustomResponse(await _mediator.Send(command));
    }

    [HttpPost("certificado")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EmitirCertificado([FromBody] EmitirCertificadoCommand certificadoCommand)
    {
        var command = new EmitirCertificadoCommand(_user.ObterUserId(), certificadoCommand.MatriculaId);

        return CustomResponse(await _mediator.Send(command));
    }

    [HttpGet("historico")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterHistorico()
    {
        var matriculas = await _alunosRepository.ObterMatriculasPorAluno(_user.ObterUserId());

        if (matriculas == null || !matriculas.Any())
            return NotFound("Nenhuma matrícula encontrada para este aluno.");

        return CustomResponse(matriculas);
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

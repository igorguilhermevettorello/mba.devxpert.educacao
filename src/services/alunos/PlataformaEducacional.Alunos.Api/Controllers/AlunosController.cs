using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacional.Alunos.Api.DTOs.Certificados;
using PlataformaEducacional.Alunos.Api.DTOs.Enderecos;
using PlataformaEducacional.Alunos.Api.DTOs.Matriculas;
using PlataformaEducacional.Alunos.Api.DTOs.Progresso;
using PlataformaEducacional.Alunos.Application.Commands;
using PlataformaEducacional.Alunos.Domain.Interfaces;
using PlataformaEducacional.Core.DomainObjects;
using PlataformaEducacional.WebApi.Core.Controllers;
using PlataformaEducacional.WebApi.Core.User;

namespace PlataformaEducacional.Alunos.Api.Controllers;

[Authorize]
[Route("api/alunos")]
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

    #region > ALUNO <
    [Tags("1. Matrículas")]
    [HttpPost("matriculas")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RealizarMatricula([FromBody] MatricularAlunoDTO model)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var usuarioId = _user.ObterUserId();
        var ehAdministrador = _user.PossuiRole("Administrador");

        if (!ehAdministrador && model.AlunoId != usuarioId)
            return Forbid();

        var command = new RealizarMatriculaCommand(model.AlunoId, model.CursoId);
        var result = await _mediator.Send(command);

        if (!result.IsValid)
            return CustomResponse(result);

        return Created();
    }

    [Tags("2. Progresso de Aulas")]
    [HttpPost("progresso")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegistrarProgresso([FromBody] RegistrarProgressoDTO progressoDto)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var command = new RegistrarProgressoCommand(_user.ObterUserId(), progressoDto.AulaId);

        return CustomResponse(await _mediator.Send(command));
    }

    [Tags("3. Certificados")]
    [HttpPost("certificado")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EmitirCertificado([FromBody] EmitirCertificadoDTO certificadoDto)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var command = new EmitirCertificadoCommand(_user.ObterUserId(), certificadoDto.MatriculaId);

        return CustomResponse(await _mediator.Send(command));
    }

    [Tags("4. Histórico")]
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


    [Tags("5. Endereço")]
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

    [Tags("5. Endereço")]
    [HttpPost("endereco")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AdicionarEndereco([FromBody] AdicionarEnderecoDTO enderecoDto)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var endereco = new AdicionarEnderecoCommand(
            enderecoDto.Logradouro,
            enderecoDto.Numero,
            enderecoDto.Complemento,
            enderecoDto.Bairro,
            enderecoDto.Cep,
            enderecoDto.Cidade,
            enderecoDto.Estado
        );
        endereco.AlunoId = _user.ObterUserId();
        return CustomResponse(await _mediator.Send(endereco));
    }

    [Tags("6. Matriculas por ID")]
    [HttpGet("matriculas/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterMatriculaPorId(Guid matriculaId)
    {
        var matricula = await _alunosRepository.ObterMatriculaPorId(matriculaId);

        if (matricula is null)
            return NotFound();

        var usuarioId = _user.ObterUserId();
        var ehAdministrador = _user.PossuiRole("Administrador");

        if (!ehAdministrador && matricula.AlunoId != usuarioId)
            return Forbid();

        return CustomResponse(matricula);
    }

    [Tags("7. Matrículas pendentes")]
    [HttpGet("pendentes/{alunoId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterMatriculasPendentes(Guid alunoId)
    {
        var ehAdministrador = _user.PossuiRole("Administrador");

        if (!ehAdministrador && alunoId != _user.ObterUserId())
            return Forbid();

        var matriculas = await _alunosRepository.ObterMatriculasPendentesPorAluno(alunoId);

        if (matriculas == null || !matriculas.Any())
            return NotFound("Nenhuma matrícula pendente encontrada para este aluno.");

        return CustomResponse(matriculas);
    }
    #endregion

    #region > ADMINISTRADOR <
    [Tags("8. Administrador")]
    [HttpGet("admin")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListarAlunos()
    {
        var alunos = await _alunosRepository.ObterTodos();
        return CustomResponse(alunos);
    }

    [Tags("8. Administrador")]
    [HttpGet("admin/{alunoId:guid}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterAlunoPorId(Guid alunoId)
    {
        var aluno = await _alunosRepository.ObterPorId(alunoId);

        if (aluno is null)
            return NotFound();

        return CustomResponse(aluno);
    }

    [Tags("8. Administrador")]
    [HttpGet("admin/{alunoId:guid}/historico")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterHistoricoPorAluno(Guid alunoId)
    {
        var matriculas = await _alunosRepository.ObterMatriculasPorAluno(alunoId);

        if (matriculas == null || !matriculas.Any())
            return NotFound("Nenhuma matrícula encontrada para este aluno.");

        return CustomResponse(matriculas);
    }

    [Tags("8. Administrador")]
    [HttpGet("admin/{alunoId:guid}/pendentes")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPendentesPorAluno(Guid alunoId)
    {
        var matriculas = await _alunosRepository.ObterMatriculasPendentesPorAluno(alunoId);

        if (matriculas == null || !matriculas.Any())
            return NotFound("Nenhuma matrícula pendente encontrada para este aluno.");

        return CustomResponse(matriculas);
    }
    #endregion  

}

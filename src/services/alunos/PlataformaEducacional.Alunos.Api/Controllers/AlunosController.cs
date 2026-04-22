using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacional.Alunos.Api.DTOs.Enderecos;
using PlataformaEducacional.Alunos.Api.DTOs.Matriculas;
using PlataformaEducacional.Alunos.Api.DTOs.Progresso;
using PlataformaEducacional.Alunos.Application.Commands;
using PlataformaEducacional.Alunos.Domain.Interfaces;
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

        var retorno = new { Success = true, Message = "Matricula realizada com sucesso" };
        return CreatedAtAction(nameof(ObterMatriculaPorId), new { id = command.AggregateId }, retorno);
    }

    [Tags("2. Progresso de Aulas")]
    [HttpPost("progresso")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegistrarProgresso([FromBody] RegistrarProgressoDTO progressoDto)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var matricula = await _alunosRepository.ObterMatriculaPorId(progressoDto.MatriculaId);

        if (matricula == null)
            return BadRequest();

        var usuarioId = _user.ObterUserId();
        var ehAdministrador = _user.PossuiRole("Administrador");

        if (!ehAdministrador && matricula.AlunoId != usuarioId)
            return Forbid();

        var command = new RegistrarProgressoCommand(progressoDto.MatriculaId, progressoDto.AulaId);

        return CustomResponse(await _mediator.Send(command));
    }

    [Tags("3. Certificados")]
    [HttpPost("matriculas/{matriculaId:guid}/certificados")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EmitirCertificado(Guid alunoId, Guid matriculaId)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var matricula = await _alunosRepository.ObterMatriculaPorId(matriculaId);

        if (matricula == null)
            return BadRequest();

        var usuarioId = _user.ObterUserId();
        var ehAdministrador = _user.PossuiRole("Administrador");

        if (!ehAdministrador && matricula.AlunoId != usuarioId)
            return Forbid();

        var command = new EmitirCertificadoCommand(matriculaId);

        return CustomResponse(await _mediator.Send(command));
    }

    [Tags("4. Histórico")]
    [HttpGet("{id:guid}/historico")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterHistoricoPorAlunoId(Guid id)
    {
        var aluno = await _alunosRepository.ObterPorId(id);

        if (aluno == null)
            return BadRequest();

        var usuarioId = _user.ObterUserId();
        var ehAdministrador = _user.PossuiRole("Administrador");

        if (!ehAdministrador && aluno.Id != usuarioId)
            return Forbid();

        var matriculas = await _alunosRepository.ObterMatriculasPorAluno(id);

        if (matriculas == null || !matriculas.Any())
            return NotFound("Nenhuma matrícula encontrada para este aluno.");

        return CustomResponse(matriculas);
    }


    [Tags("5. Endereço")]
    [HttpGet("{id:guid}/endereco")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterEnderecoPorAlunoId(Guid id)
    {
        var aluno = await _alunosRepository.ObterPorId(id);

        if (aluno == null)
            return BadRequest();

        var usuarioId = _user.ObterUserId();
        var ehAdministrador = _user.PossuiRole("Administrador");

        if (!ehAdministrador && aluno.Id != usuarioId)
            return Forbid();

        var address = await _alunosRepository.ObterEnderecoPorAlunoId(id);

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

        var aluno = await _alunosRepository.ObterPorId(enderecoDto.AlunoId);

        if (aluno == null)
            return BadRequest();

        var usuarioId = _user.ObterUserId();
        var ehAdministrador = _user.PossuiRole("Administrador");

        if (!ehAdministrador && aluno.Id != usuarioId)
            return Forbid();

        var endereco = new AdicionarEnderecoCommand(
            enderecoDto.AlunoId,
            enderecoDto.Logradouro,
            enderecoDto.Numero,
            enderecoDto.Complemento,
            enderecoDto.Bairro,
            enderecoDto.Cep,
            enderecoDto.Cidade,
            enderecoDto.Estado
        );

        return CustomResponse(await _mediator.Send(endereco));
    }

    [Tags("6. Matriculas por ID")]
    [HttpGet("matriculas/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterMatriculaPorId(Guid id)
    {
        var matricula = await _alunosRepository.ObterMatriculaPorId(id);

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

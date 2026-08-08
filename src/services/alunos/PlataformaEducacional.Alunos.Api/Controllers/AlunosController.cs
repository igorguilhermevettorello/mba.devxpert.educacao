using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlataformaEducacional.Alunos.Api.DTOs.Enderecos;
using PlataformaEducacional.Alunos.Api.DTOs.Matriculas;
using PlataformaEducacional.Alunos.Api.DTOs.Progresso;
using PlataformaEducacional.Alunos.Application.Commands;
using PlataformaEducacional.Alunos.Domain.Interfaces;
using PlataformaEducacional.Alunos.Domain.Models;
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
    private readonly ILogger<AlunosController> _logger;

    public AlunosController(IAlunoRepository alunosRepository, IMediator mediator, IAspNetUser user, ILogger<AlunosController> logger)
    {
        _alunosRepository = alunosRepository;
        _mediator = mediator;
        _user = user;
        _logger = logger;
    }

    #region > ALUNO <
    [Tags("1. Matrículas")]
    [HttpPost("{alunoId:guid}/matriculas")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RealizarMatricula([FromRoute] Guid alunoId, [FromBody] MatricularAlunoDTO model)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        _logger.LogInformation("Solicitação de matrícula recebida para AlunoId {AlunoId} no Curso {CursoId}", alunoId, model.CursoId);

        var aluno = await _alunosRepository.ObterPorId(alunoId);

        if (aluno is null)
        {
            _logger.LogWarning("Tentativa de matrícula para AlunoId {AlunoId} não encontrado", alunoId);
            return NotFound();
        }

        if (!UsuarioPodeVerAluno(alunoId))
        {
            _logger.LogWarning("Acesso negado para matrícula do AlunoId {AlunoId} pelo usuário {UserId}", alunoId, _user.ObterUserId());
            return Forbid();
        }

        var command = new RealizarMatriculaCommand(alunoId, model.CursoId);
        var result = await _mediator.Send(command);

        if (!result.IsValid)
        {
            _logger.LogWarning("Matrícula não validada para AlunoId {AlunoId}: {Errors}", alunoId, string.Join(", ", result.Errors.Select(e => e.ErrorMessage)));
            return CustomResponse(result);
        }

        _logger.LogInformation("Matrícula realizada com sucesso para AlunoId {AlunoId} no Curso {CursoId}", alunoId, model.CursoId);
        var retorno = new { Success = true, Message = "Matricula realizada com sucesso" };
        return CreatedAtAction(nameof(ObterMatriculaPorId), new { id = command.AggregateId }, retorno);
    }

    [Tags("2. Progresso de Aulas")]
    [HttpPost("{alunoId:guid}/matriculas/{matriculaId:guid}/progresso-aulas")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegistrarProgresso([FromRoute] Guid alunoId, [FromRoute] Guid matriculaId, [FromBody] RegistrarProgressoDTO progressoDto)
    {
        if (!ModelState.IsValid)
            return CustomResponse(ModelState);

        _logger.LogInformation("Registro de progresso solicitado para AlunoId {AlunoId}, MatriculaId {MatriculaId}, AulaId {AulaId}", alunoId, matriculaId, progressoDto.AulaId);

        var aluno = await _alunosRepository.ObterPorId(alunoId);

        if (aluno is null || !aluno.Matriculas.Any(x => x.Id == matriculaId))
        {
            _logger.LogWarning("Matrícula {MatriculaId} não encontrada para AlunoId {AlunoId}", matriculaId, alunoId);
            return NotFound();
        }

        if (!UsuarioPodeVerAluno(alunoId))
        {
            _logger.LogWarning("Acesso negado para registro de progresso do AlunoId {AlunoId}", alunoId);
            return Forbid();
        }

        var command = new RegistrarProgressoCommand(alunoId, progressoDto.AulaId);

        var result = await _mediator.Send(command);

        if (!result.IsValid)
            _logger.LogWarning("Progresso não validado para AlunoId {AlunoId}: {Errors}", alunoId, string.Join(", ", result.Errors.Select(e => e.ErrorMessage)));
        else
            _logger.LogInformation("Progresso registrado com sucesso para AlunoId {AlunoId}, AulaId {AulaId}", alunoId, progressoDto.AulaId);

        return CustomResponse(result);
    }

    [Tags("3. Certificados")]
    [HttpPost("{alunoId:guid}/matriculas/{matriculaId:guid}/certificados")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EmitirCertificado([FromRoute] Guid alunoId, [FromRoute] Guid matriculaId)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        _logger.LogInformation("Emissão de certificado solicitada para AlunoId {AlunoId}, MatriculaId {MatriculaId}", alunoId, matriculaId);

        var aluno = await _alunosRepository.ObterPorId(alunoId);

        if (aluno is null || !aluno.Matriculas.Any(x => x.Id == matriculaId))
        {
            _logger.LogWarning("Matrícula {MatriculaId} não encontrada para emissão de certificado do AlunoId {AlunoId}", matriculaId, alunoId);
            return NotFound();
        }

        if (!UsuarioPodeVerAluno(alunoId))
        {
            _logger.LogWarning("Acesso negado para emissão de certificado do AlunoId {AlunoId}", alunoId);
            return Forbid();
        }

        var command = new EmitirCertificadoCommand(matriculaId);

        var result = await _mediator.Send(command);

        if (!result.IsValid)
            _logger.LogWarning("Certificado não emitido para MatriculaId {MatriculaId}: {Errors}", matriculaId, string.Join(", ", result.Errors.Select(e => e.ErrorMessage)));
        else
            _logger.LogInformation("Certificado emitido com sucesso para AlunoId {AlunoId}, MatriculaId {MatriculaId}", alunoId, matriculaId);

        return CustomResponse(result);
    }

    [Tags("4. Histórico")]
    [HttpGet("{alunoId:guid}/historico")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterHistoricoPorAlunoId([FromRoute] Guid alunoId)
    {
        var aluno = await _alunosRepository.ObterPorId(alunoId);

        if (aluno is null)
            return NotFound();

        if (!UsuarioPodeVerAluno(alunoId))
            return Forbid();

        var matriculas = await _alunosRepository.ObterMatriculasPorAluno(alunoId);

        return CustomResponse(aluno.Matriculas);
    }


    [Tags("5. Endereço")]
    [HttpGet("{alunoId:guid}/endereco")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterEnderecoPorAlunoId([FromRoute] Guid alunoId)
    {
        var aluno = await _alunosRepository.ObterPorId(alunoId);

        if (aluno is null)
            return NotFound();

        if (!UsuarioPodeVerAluno(alunoId))
            return Forbid();

        var endereco = await _alunosRepository.ObterEnderecoPorAlunoId(alunoId);

        if (endereco is null)
            return NotFound();

        return CustomResponse(endereco);
    }

    [Tags("5. Endereço")]
    [HttpPost("{alunoId:guid}/endereco")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AdicionarEndereco([FromRoute] Guid alunoId, [FromBody] AdicionarEnderecoDTO enderecoDto)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        _logger.LogInformation("Adição de endereço solicitada para AlunoId {AlunoId}", alunoId);

        var aluno = await _alunosRepository.ObterPorId(alunoId);

        if (aluno is null)
        {
            _logger.LogWarning("AlunoId {AlunoId} não encontrado para adição de endereço", alunoId);
            return NotFound();
        }

        if (!UsuarioPodeVerAluno(alunoId))
        {
            _logger.LogWarning("Acesso negado para adição de endereço do AlunoId {AlunoId}", alunoId);
            return Forbid();
        }

        var endereco = new AdicionarEnderecoCommand(
            alunoId,
            enderecoDto.Logradouro,
            enderecoDto.Numero,
            enderecoDto.Complemento,
            enderecoDto.Bairro,
            enderecoDto.Cep,
            enderecoDto.Cidade,
            enderecoDto.Estado
        );

        var result = await _mediator.Send(endereco);

        if (!result.IsValid)
            _logger.LogWarning("Endereço não adicionado para AlunoId {AlunoId}: {Errors}", alunoId, string.Join(", ", result.Errors.Select(e => e.ErrorMessage)));
        else
            _logger.LogInformation("Endereço adicionado com sucesso para AlunoId {AlunoId}, CEP {Cep}", alunoId, enderecoDto.Cep);

        return CustomResponse(result);
    }

    [Tags("6. Matriculas por ID")]
    [HttpGet("matriculas/{matriculaId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterMatriculaPorId([FromRoute] Guid matriculaId)
    {
        var matricula = await _alunosRepository.ObterMatriculaPorId(matriculaId);

        if (matricula is null)
            return NotFound();

        if (!UsuarioPodeVerAluno(matricula.AlunoId))
            return Forbid();

        return CustomResponse(matricula);
    }

    [Tags("7. Matrículas pendentes")]
    [HttpGet("{alunoId:guid}/matriculas/pendentes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterMatriculasPendentes([FromRoute] Guid alunoId)
    {
        var aluno = await _alunosRepository.ObterPorId(alunoId);

        if (aluno is null)
            return NotFound();

        if (!UsuarioPodeVerAluno(alunoId))
            return Forbid();

        var matriculas = await _alunosRepository.ObterMatriculasPendentesPorAluno(alunoId);

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

    private bool UsuarioPodeVerAluno(Guid alunoId)
    {
        if (_user.PossuiRole("Administrador"))
            return true;

        var usuarioId = _user.ObterUserId();
        return usuarioId == alunoId;
    }

}

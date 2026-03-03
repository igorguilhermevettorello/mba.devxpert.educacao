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

    [Tags("1. Matrículas")]
    [HttpPost("matricula")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RealizarMatricula([FromBody] MatricularAlunoDTO model)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var alunoId = _user.ObterUserId();

        if (alunoId != model.AlunoId)
            throw new DomainException("Aluno não identificado"); 

        var command = new RealizarMatriculaCommand(alunoId, model.CursoId, model.Valor);
        var result = await _mediator.Send(command);

        return CustomResponse(result);
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
}

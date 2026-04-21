using FluentValidation.Results;
using MediatR;
using PlataformaEducacional.Alunos.Application.Events;
using PlataformaEducacional.Alunos.Application.Services;
using PlataformaEducacional.Alunos.Domain.Interfaces;
using PlataformaEducacional.Alunos.Domain.Models;
using PlataformaEducacional.Core.Messages;
using PlataformaEducacional.MessageBus;
using PlataformaEducacional.WebApi.Core.Enumerators;

namespace PlataformaEducacional.Alunos.Application.Commands;

public class AlunoCommandHandler : CommandHandler,
    IRequestHandler<RegistrarAlunoCommand, ValidationResult>,
    IRequestHandler<AdicionarEnderecoCommand, ValidationResult>,
    IRequestHandler<RealizarMatriculaCommand, ValidationResult>,
    IRequestHandler<RegistrarProgressoCommand, ValidationResult>,
    IRequestHandler<EmitirCertificadoCommand, ValidationResult>
{
    private readonly IAlunoRepository _alunoRepository;
    private readonly IMessageBus _bus;
    private readonly IConteudoService _conteudoService;

    public AlunoCommandHandler(IAlunoRepository alunoRepository, IMessageBus bus, IConteudoService conteudoService)
    {
        _alunoRepository = alunoRepository;
        _bus = bus;
        _conteudoService = conteudoService;
    }

    public async Task<ValidationResult> Handle(AdicionarEnderecoCommand message, CancellationToken cancellationToken)
    {
        if (!message.IsValid()) return message.ValidationResult;

        var endereco = new Endereco(message.Logradouro, message.Numero, message.Complemento, message.Bairro, message.Cep, message.Cidade, message.Estado, message.AlunoId);
        _alunoRepository.AdicionarEndereco(endereco);

        return await PersistData(_alunoRepository.UnitOfWork);
    }

    public async Task<ValidationResult> Handle(RegistrarAlunoCommand message, CancellationToken cancellationToken)
    {
        if (!message.IsValid()) return message.ValidationResult;

        var aluno = new Aluno(message.Id, message.Nome, message.Email, message.Cpf);
        var alunoExistente = await _alunoRepository.ObterPorCpf(aluno.Cpf.Numero);

        if (alunoExistente != null)
        {
            AddError("Este CPF já está em uso.");
            return ValidationResult;
        }

        _alunoRepository.Adicionar(aluno);

        aluno.AddEvent(new AlunoRegistradoEvent(message.Id, message.Nome, message.Email, message.Cpf));

        return await PersistData(_alunoRepository.UnitOfWork);
    }

    public async Task<ValidationResult> Handle(RealizarMatriculaCommand message, CancellationToken cancellationToken)
    {
        if (!message.IsValid())
            return message.ValidationResult;

        var aluno = await _alunoRepository.ObterPorId(message.AlunoId);

        if (aluno == null)
        {
            AddError("Aluno não encontrado.");
            return ValidationResult;
        }

        var cursoExiste = await _conteudoService.CursoExisteAsync(message.CursoId);
        if (!cursoExiste)
        {
            AddError("Curso não encontrado ou indisponível.");
            return ValidationResult;
        }

        if (aluno.Matriculas.Any(x => x.CursoId == message.CursoId))
        {
            AddError($"Aluno já possui matricula no curso {message.CursoId}");
            return ValidationResult;
        }

        var matricula = new Matricula(message.AlunoId, message.CursoId);
        _alunoRepository.AdicionarMatricula(matricula);

        var result = await PersistData(_alunoRepository.UnitOfWork);
        return result;
    }

    public async Task<ValidationResult> Handle(RegistrarProgressoCommand message, CancellationToken cancellationToken)
    {
        if (!message.IsValid()) return message.ValidationResult;

        var cursoIdRelacionado = await _conteudoService.ObterCursoIdPorAulaAsync(message.AulaId);
        if (cursoIdRelacionado == null)
        {
            AddError("A aula informada não foi encontrada na API de Conteúdos.");
            return ValidationResult;
        }

        var matriculas = await _alunoRepository.ObterMatriculasPorAluno(message.AlunoId);
        var matriculaAtiva = matriculas.FirstOrDefault(m => m.CursoId.Equals(cursoIdRelacionado.Value) && m.Status == StatusMatricula.Ativa);
        if (matriculaAtiva is null)
        {
            AddError("Aluno não possui matrícula ativa para o curso desta aula.");
            return ValidationResult;
        }

        if (matriculaAtiva.ProgressoAulas.Any(p => p.AulaId == message.AulaId))
        {
            AddError("O progresso desta aula já foi registrado anteriormente.");
            return ValidationResult;
        }

        var progresso = new ProgressoAula(matriculaAtiva.Id, message.AulaId);
        _alunoRepository.AdicionarProgresso(progresso);

        return await PersistData(_alunoRepository.UnitOfWork);
    }

    public async Task<ValidationResult> Handle(EmitirCertificadoCommand message, CancellationToken cancellationToken)
    {
        if (!message.IsValid()) return message.ValidationResult;

        var matricula = await _alunoRepository.ObterMatriculaPorId(message.MatriculaId);

        if (matricula == null || matricula.AlunoId != message.AlunoId)
        {
            AddError("Matrícula inválida ou não pertence ao aluno.");
            return ValidationResult;
        }

        if (matricula.Status != StatusMatricula.Ativa && matricula.Status != StatusMatricula.Concluida)
        {
            AddError("A matrícula precisa estar ativa ou concluída para emitir o certificado.");
            return ValidationResult;
        }

        if (matricula.Certificado != null)
        {
            AddError("Certificado já emitido para esta matrícula.");
            return ValidationResult;
        }

        var totalAulasCurso = await _conteudoService.ObterTotalAulasPorCursoAsync(matricula.CursoId);

        if (totalAulasCurso == 0 || matricula.ProgressoAulas.Count < totalAulasCurso)
        {
            AddError($"O aluno ainda não concluiu todas as {totalAulasCurso} aulas deste curso.");
            return ValidationResult;
        }

        var certificado = new Certificado(matricula.Id);
        matricula.Concluir();
        _alunoRepository.AttachMatricula(matricula);
        _alunoRepository.AdicionarCertifficado(certificado);

        return await PersistData(_alunoRepository.UnitOfWork);
    }
}

using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<AlunoCommandHandler> _logger;

    public AlunoCommandHandler(IAlunoRepository alunoRepository, IMessageBus bus, IConteudoService conteudoService, ILogger<AlunoCommandHandler> logger)
    {
        _alunoRepository = alunoRepository;
        _bus = bus;
        _conteudoService = conteudoService;
        _logger = logger;
    }

    public async Task<ValidationResult> Handle(AdicionarEnderecoCommand message, CancellationToken cancellationToken)
    {
        if (!message.IsValid()) return message.ValidationResult;

        var endereco = new Endereco(message.Logradouro, message.Numero, message.Complemento, message.Bairro, message.Cep, message.Cidade, message.Estado, message.AlunoId);
        _alunoRepository.AdicionarEndereco(endereco);

        var result = await PersistData(_alunoRepository.UnitOfWork);

        if (result.IsValid)
            _logger.LogInformation("Endereço adicionado com sucesso para AlunoId {AlunoId}", message.AlunoId);

        return result;
    }

    public async Task<ValidationResult> Handle(RegistrarAlunoCommand message, CancellationToken cancellationToken)
    {
        if (!message.IsValid()) return message.ValidationResult;

        var aluno = new Aluno(message.Id, message.Nome, message.Email, message.Cpf);
        var alunoExistente = await _alunoRepository.ObterPorCpf(aluno.Cpf.Numero);

        if (alunoExistente != null)
        {
            _logger.LogWarning("Tentativa de registrar aluno com CPF duplicado: {Cpf}", aluno.Cpf.Numero);
            AddError("Este CPF já está em uso.");
            return ValidationResult;
        }

        _alunoRepository.Adicionar(aluno);

        aluno.AddEvent(new AlunoRegistradoEvent(message.Id, message.Nome, message.Email, message.Cpf));

        var result = await PersistData(_alunoRepository.UnitOfWork);

        if (result.IsValid)
            _logger.LogInformation("Aluno registrado com sucesso - AlunoId {AlunoId}, Nome {Nome}", message.Id, message.Nome);

        return result;
    }

    public async Task<ValidationResult> Handle(RealizarMatriculaCommand message, CancellationToken cancellationToken)
    {
        if (!message.IsValid())
            return message.ValidationResult;

        var aluno = await _alunoRepository.ObterPorId(message.AlunoId);

        if (aluno == null)
        {
            _logger.LogWarning("Matrícula não realizada - Aluno {AlunoId} não encontrado", message.AlunoId);
            AddError("Aluno não encontrado.");
            return ValidationResult;
        }

        var cursoExiste = await _conteudoService.CursoExisteAsync(message.CursoId);
        if (!cursoExiste)
        {
            _logger.LogWarning("Matrícula não realizada - Curso {CursoId} não encontrado ou indisponível para AlunoId {AlunoId}", message.CursoId, message.AlunoId);
            AddError("Curso não encontrado ou indisponível.");
            return ValidationResult;
        }

        if (aluno.Matriculas.Any(x => x.CursoId == message.CursoId))
        {
            _logger.LogWarning("Matrícula não realizada - Aluno {AlunoId} já possui matrícula no Curso {CursoId}", message.AlunoId, message.CursoId);
            AddError($"Aluno já possui matricula no curso {message.CursoId}");
            return ValidationResult;
        }

        var matricula = new Matricula(message.AlunoId, message.CursoId);
        _alunoRepository.AdicionarMatricula(matricula);

        var result = await PersistData(_alunoRepository.UnitOfWork);

        if (result.IsValid)
            _logger.LogInformation("Matrícula realizada com sucesso - AlunoId {AlunoId}, CursoId {CursoId}", message.AlunoId, message.CursoId);

        return result;
    }

    public async Task<ValidationResult> Handle(RegistrarProgressoCommand message, CancellationToken cancellationToken)
    {
        if (!message.IsValid()) return message.ValidationResult;

        var cursoIdRelacionado = await _conteudoService.ObterCursoIdPorAulaAsync(message.AulaId);
        if (cursoIdRelacionado == null)
        {
            _logger.LogWarning("Progresso não registrado - Aula {AulaId} não encontrada na API de Conteúdos para AlunoId {AlunoId}", message.AulaId, message.AlunoId);
            AddError("A aula informada não foi encontrada na API de Conteúdos.");
            return ValidationResult;
        }

        var matriculas = await _alunoRepository.ObterMatriculasPorAluno(message.AlunoId);
        var matriculaAtiva = matriculas.FirstOrDefault(m => m.CursoId.Equals(cursoIdRelacionado.Value) && m.Status == StatusMatricula.Ativa);
        if (matriculaAtiva is null)
        {
            _logger.LogWarning("Progresso não registrado - Aluno {AlunoId} não possui matrícula ativa para Aula {AulaId}", message.AlunoId, message.AulaId);
            AddError("Aluno não possui matrícula ativa para o curso desta aula.");
            return ValidationResult;
        }

        if (matriculaAtiva.ProgressoAulas.Any(p => p.AulaId == message.AulaId))
        {
            _logger.LogWarning("Progresso não registrado - Aula {AulaId} já registrada para MatriculaId {MatriculaId}", message.AulaId, matriculaAtiva.Id);
            AddError("O progresso desta aula já foi registrado anteriormente.");
            return ValidationResult;
        }

        var progresso = new ProgressoAula(matriculaAtiva.Id, message.AulaId);
        _alunoRepository.AdicionarProgresso(progresso);

        var result = await PersistData(_alunoRepository.UnitOfWork);

        if (result.IsValid)
            _logger.LogInformation("Progresso registrado com sucesso - AlunoId {AlunoId}, AulaId {AulaId}", message.AlunoId, message.AulaId);

        return result;
    }

    public async Task<ValidationResult> Handle(EmitirCertificadoCommand message, CancellationToken cancellationToken)
    {
        if (!message.IsValid()) return message.ValidationResult;

        var matricula = await _alunoRepository.ObterMatriculaPorId(message.MatriculaId);

        if (matricula.Status != StatusMatricula.Ativa && matricula.Status != StatusMatricula.Concluida)
        {
            _logger.LogWarning("Certificado não emitido - Matrícula {MatriculaId} com status {Status} (requer Ativa ou Concluída)", message.MatriculaId, matricula.Status);
            AddError("A matrícula precisa estar ativa ou concluída para emitir o certificado.");
            return ValidationResult;
        }

        if (matricula.Certificado != null)
        {
            _logger.LogWarning("Certificado não emitido - Certificado já existe para MatriculaId {MatriculaId}", message.MatriculaId);
            AddError("Certificado já emitido para esta matrícula.");
            return ValidationResult;
        }

        var totalAulasCurso = await _conteudoService.ObterTotalAulasPorCursoAsync(matricula.CursoId);

        if (totalAulasCurso == 0 || matricula.ProgressoAulas.Count < totalAulasCurso)
        {
            _logger.LogWarning("Certificado não emitido - AlunoId {AlunoId} completou {ProgressoCount} de {TotalAulas} aulas do Curso {CursoId}",
                matricula.AlunoId, matricula.ProgressoAulas.Count, totalAulasCurso, matricula.CursoId);
            AddError($"O aluno ainda não concluiu todas as {totalAulasCurso} aulas deste curso.");
            return ValidationResult;
        }

        var certificado = new Certificado(matricula.Id);
        matricula.Concluir();
        _alunoRepository.AttachMatricula(matricula);
        _alunoRepository.AdicionarCertifficado(certificado);

        var result = await PersistData(_alunoRepository.UnitOfWork);

        if (result.IsValid)
            _logger.LogInformation("Certificado emitido com sucesso - AlunoId {AlunoId}, MatriculaId {MatriculaId}, CursoId {CursoId}",
                matricula.AlunoId, message.MatriculaId, matricula.CursoId);

        return result;
    }
}

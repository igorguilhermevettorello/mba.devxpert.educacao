using FluentValidation.Results;
using MediatR;
using PlataformaEducacional.Alunos.Application.Events;
using PlataformaEducacional.Alunos.Domain.Interfaces;
using PlataformaEducacional.Alunos.Domain.Models;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Alunos.Application.Commands;

public class AlunoCommandHandler : CommandHandler,
    IRequestHandler<RegistrarAlunoCommand, ValidationResult>,
    IRequestHandler<AdicionarEnderecoCommand, ValidationResult>,
    IRequestHandler<RealizarMatriculaCommand, ValidationResult>,
    IRequestHandler<RegistrarProgressoCommand, ValidationResult>,
    IRequestHandler<EmitirCertificadoCommand, ValidationResult>
{
    private readonly IAlunoRepository _alunoRepository;

    public AlunoCommandHandler(IAlunoRepository alunoRepository)
    {
        _alunoRepository = alunoRepository;
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
        if (!message.IsValid()) return message.ValidationResult;

        var aluno = await _alunoRepository.ObterPorId(message.AlunoId);

        if (aluno == null)
        {
            AddError("Aluno não encontrado.");
            return ValidationResult;
        }

        var matricula = new Matricula(message.AlunoId, message.CursoId, message.Valor);
        
        _alunoRepository.AdicionarMatricula(matricula);

        return await PersistData(_alunoRepository.UnitOfWork);
    }

    public async Task<ValidationResult> Handle(RegistrarProgressoCommand message, CancellationToken cancellationToken)
    {
        if (!message.IsValid()) return message.ValidationResult;

        var matriculas = await _alunoRepository.ObterMatriculasPorAluno(message.AlunoId);
        var matriculaAtiva = matriculas.FirstOrDefault(m => m.Status == Domain.Models.EnumStatusMatricula.Ativa);

        if (matriculaAtiva == null)
        {
            AddError("Aluno não possui matrícula ativa para este curso.");
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

        if (matricula.Status != Domain.Models.EnumStatusMatricula.Ativa && matricula.Status != Domain.Models.EnumStatusMatricula.Concluida)
        {
             AddError("A matrícula precisa estar ativa ou concluída para emitir o certificado.");
             return ValidationResult;
        }

        if (matricula.Certificado != null)
        {
             AddError("Certificado já emitido para esta matrícula.");
             return ValidationResult;
        }

        // Em um cenário real, aqui entraria a validação com a API de Conteúdo
        // para checar se a quantidade de aulas no progresso == quantidade de aulas do curso
        
        var certificado = new Certificado(matricula.Id);
        matricula.Concluir();

        // O Entity Framework vai persistir o Certificado por causa do relacionamento
        // Mas se precisar, podemos dicionar um método _alunoRepository.AdicionarCertificado(certificado) na interface

        return await PersistData(_alunoRepository.UnitOfWork);
    }
}

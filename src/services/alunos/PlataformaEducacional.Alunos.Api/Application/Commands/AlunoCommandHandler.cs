using FluentValidation.Results;
using MediatR;
using PlataformaEducacional.Alunos.Api.Application.Events;
using PlataformaEducacional.Alunos.Api.Interfaces;
using PlataformaEducacional.Alunos.Api.Models;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Alunos.Api.Application.Commands;

public class AlunoCommandHandler : CommandHandler,
    IRequestHandler<RegistrarAlunoCommand, ValidationResult>,
    IRequestHandler<AdicionarEnderecoCommand, ValidationResult>
{
    private readonly IAlunoRepository _alunoRepository;

    public AlunoCommandHandler(IAlunoRepository alunoRepository)
    {
        _alunoRepository = alunoRepository;
    }

    public async Task<ValidationResult> Handle(AdicionarEnderecoCommand message, CancellationToken cancellationToken)
    {
        if (!message.IsValid()) return message.ValidationResult;

        var endereco = new Endereco(message);
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
}

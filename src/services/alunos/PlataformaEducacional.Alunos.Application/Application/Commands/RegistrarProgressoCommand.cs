using FluentValidation;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Alunos.Application.Commands;

public class RegistrarProgressoCommand : Command
{
    public Guid AlunoId { get; private set; }
    public Guid AulaId { get; private set; }

    public RegistrarProgressoCommand(Guid alunoId, Guid aulaId)
    {
        AlunoId = alunoId;
        AulaId = aulaId;
    }

    public override bool IsValid()
    {
        ValidationResult = new RegistrarProgressoValidation().Validate(this);
        return ValidationResult.IsValid;
    }
}

public class RegistrarProgressoValidation : AbstractValidator<RegistrarProgressoCommand>
{
    public RegistrarProgressoValidation()
    {
        RuleFor(c => c.AlunoId)
            .NotEqual(Guid.Empty)
            .WithMessage("Id do aluno inválido");

        RuleFor(c => c.AulaId)
            .NotEqual(Guid.Empty)
            .WithMessage("Id da aula inválido");
    }
}

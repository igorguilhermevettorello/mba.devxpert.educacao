using FluentValidation;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Alunos.Application.Commands;

public class EmitirCertificadoCommand : Command
{
    public Guid AlunoId { get; private set; }
    public Guid MatriculaId { get; private set; }

    public EmitirCertificadoCommand(Guid alunoId, Guid matriculaId)
    {
        AlunoId = alunoId;
        MatriculaId = matriculaId;
    }

    public override bool IsValid()
    {
        ValidationResult = new EmitirCertificadoValidation().Validate(this);
        return ValidationResult.IsValid;
    }
}

public class EmitirCertificadoValidation : AbstractValidator<EmitirCertificadoCommand>
{
    public EmitirCertificadoValidation()
    {
        RuleFor(c => c.AlunoId)
            .NotEqual(Guid.Empty)
            .WithMessage("Id do aluno inválido");

        RuleFor(c => c.MatriculaId)
            .NotEqual(Guid.Empty)
            .WithMessage("Id da matrícula inválido");
    }
}

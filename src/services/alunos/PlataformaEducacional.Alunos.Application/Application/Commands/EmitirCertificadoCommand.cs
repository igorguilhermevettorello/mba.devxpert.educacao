using FluentValidation;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Alunos.Application.Commands;

public class EmitirCertificadoCommand : Command
{
    public Guid MatriculaId { get; private set; }

    public EmitirCertificadoCommand(Guid matriculaId)
    {
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
        RuleFor(c => c.MatriculaId)
            .NotEqual(Guid.Empty)
            .WithMessage("Id da matrícula inválido");
    }
}

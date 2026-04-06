using FluentValidation;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Alunos.Application.Commands;

public class RealizarMatriculaCommand : Command
{
    public RealizarMatriculaCommand(Guid alunoId, Guid cursoId)
    {
        AlunoId = alunoId;
        CursoId = cursoId;
    }

    public Guid AlunoId { get; private set; }
    public Guid CursoId { get; private set; }

    public override bool IsValid()
    {
        ValidationResult = new RealizarMatriculaValidation().Validate(this);
        return ValidationResult.IsValid;
    }
}

public class RealizarMatriculaValidation : AbstractValidator<RealizarMatriculaCommand>
{
    public RealizarMatriculaValidation()
    {
        RuleFor(c => c.AlunoId)
            .NotEqual(Guid.Empty)
            .WithMessage("Id do aluno inválido");

        RuleFor(c => c.CursoId)
            .NotEqual(Guid.Empty)
            .WithMessage("Id do curso inválido");
    }
}

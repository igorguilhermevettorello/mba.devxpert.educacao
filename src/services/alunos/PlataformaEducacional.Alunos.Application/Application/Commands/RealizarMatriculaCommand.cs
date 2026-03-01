using FluentValidation;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Alunos.Application.Commands;

public class RealizarMatriculaCommand : Command
{
    public Guid AlunoId { get; private set; }
    public Guid CursoId { get; private set; }
    public decimal Valor { get; private set; }

    public RealizarMatriculaCommand(Guid alunoId, Guid cursoId, decimal valor)
    {
        AlunoId = alunoId;
        CursoId = cursoId;
        Valor = valor;
    }

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

        RuleFor(c => c.Valor)
            .GreaterThan(0)
            .WithMessage("O valor da matrícula precisa ser maior que 0");
    }
}

using FluentValidation;
using PlataformaEducacional.Conteudo.Application.Commands.Cursos;

namespace PlataformaEducacional.Conteudo.Application.Validators
{
    public class AtivarCursoCommandValidator : AbstractValidator<AtivarCursoCommand>
    {
        public AtivarCursoCommandValidator()
        {
            RuleFor(x => x.CursoId)
                .NotEmpty()
                .WithMessage("Id do curso é obrigatório.");
        }
    }
}

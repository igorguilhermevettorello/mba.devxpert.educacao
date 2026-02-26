using FluentValidation;
using PlataformaEducacional.Conteudo.Application.Commands.Cursos;

namespace PlataformaEducacional.Conteudo.Application.Validators
{
    public class AtivarCursoCommandValidator : AbstractValidator<AtivarCursoCommand>
    {
        public AtivarCursoCommandValidator()
        {
            RuleFor(x => x.CursoId)
                .Empty()
                .WithMessage("Id do curso é obrigatório.");
        }
    }
}

using FluentValidation;
using PlataformaEducacional.Conteudo.Application.Commands.Cursos;

namespace PlataformaEducacional.Conteudo.Application.Validators
{
    public class InativarCursoCommandValidator : AbstractValidator<InativarCursoCommand>
    {
        public InativarCursoCommandValidator()
        {
            RuleFor(x => x.CursoId)
                .NotEmpty()
                .WithMessage("Id do curso é obrigatório.");
        }
    }
}

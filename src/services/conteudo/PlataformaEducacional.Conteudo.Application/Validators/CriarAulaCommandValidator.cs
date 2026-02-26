using FluentValidation;
using PlataformaEducacional.Conteudo.Application.Commands.Aulas;

namespace PlataformaEducacional.Conteudo.Application.Validators
{
    public class CriarAulaCommandValidator : AbstractValidator<CriarAulaCommand>
    {
        public CriarAulaCommandValidator()
        {
            RuleFor(x => x.CursoId)
                .NotEmpty()
                .WithMessage("O ID do curso é obrigatório");

            RuleFor(x => x.Titulo)
                .NotEmpty()
                .WithMessage("Título da aula é obrigatório")
                .MaximumLength(200)
                .WithMessage("Título da aula deve ter no máximo 200 caracteres")
                .MinimumLength(3)
                .WithMessage("Título da aula deve ter no mínimo 3 caracteres");

            RuleFor(x => x.Descricao)
                .NotEmpty()
                .WithMessage("Descrição da aula é obrigatória")
                .MaximumLength(1000)
                .WithMessage("Descrição da aula deve ter no máximo 1000 caracteres")
                .MinimumLength(10)
                .WithMessage("Descrição da aula deve ter no mínimo 10 caracteres");

            RuleFor(x => x.DuracaoMinutos)
                .GreaterThan(0)
                .WithMessage("Duração deve ser maior que zero");

            RuleFor(x => x.Ordem)
                .GreaterThan(0)
                .WithMessage("Ordem deve ser maior que zero");
        }
    }
}

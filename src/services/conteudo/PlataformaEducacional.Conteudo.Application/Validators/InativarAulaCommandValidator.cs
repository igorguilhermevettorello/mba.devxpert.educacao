using FluentValidation;
using PlataformaEducacional.Conteudo.Application.Commands.Aulas;

namespace PlataformaEducacional.Conteudo.Application.Validators
{
    public class InativarAulaCommandValidator : AbstractValidator<InativarAulaCommand>
    {
        public InativarAulaCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("O ID da aula é obrigatório");
        }
    }
}

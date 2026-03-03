using FluentValidation;
using PlataformaEducacional.Conteudo.Application.Commands.Aulas;

namespace PlataformaEducacional.Conteudo.Application.Validators
{
    public class AtivarAulaCommandValidator : AbstractValidator<AtivarAulaCommand>
    {
        public AtivarAulaCommandValidator()
        {
            RuleFor(x => x.AulaId)
                .NotEmpty()
                .WithMessage("O ID da aula é obrigatório");
        }
    }
}

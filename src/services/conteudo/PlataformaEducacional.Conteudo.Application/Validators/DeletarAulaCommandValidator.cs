using FluentValidation;
using PlataformaEducacional.Conteudo.Application.Commands.Aulas;

namespace PlataformaEducacional.Conteudo.Application.Validators
{
    public class DeletarAulaCommandValidator : AbstractValidator<DeletarAulaCommand>
    {
        public DeletarAulaCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("O ID da aula é obrigatório");
        }
    }
}

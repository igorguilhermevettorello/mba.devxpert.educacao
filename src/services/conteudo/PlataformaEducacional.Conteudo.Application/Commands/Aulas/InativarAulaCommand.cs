using MediatR;
using PlataformaEducacional.Conteudo.Application.Validators;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Commands.Aulas
{
    public class InativarAulaCommand : Command, IRequest<bool>
    {
        public Guid Id { get; set; }

        public InativarAulaCommand(Guid id)
        {
            Id = id;
        }

        public override bool IsValid()
        {
            ValidationResult = new InativarAulaCommandValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}

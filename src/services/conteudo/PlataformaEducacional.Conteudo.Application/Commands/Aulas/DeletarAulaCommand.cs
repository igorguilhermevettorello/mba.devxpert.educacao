using MediatR;
using PlataformaEducacional.Conteudo.Application.Validators;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Commands.Aulas
{
    public class DeletarAulaCommand : Command, IRequest<bool>
    {
        public Guid Id { get; set; }

        public DeletarAulaCommand(Guid id)
        {
            Id = id;
        }

        public override bool IsValid()
        {
            ValidationResult = new DeletarAulaCommandValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}

using MediatR;
using PlataformaEducacional.Conteudo.Application.Validators;
using PlataformaEducacional.Core.Messages;
using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacional.Conteudo.Application.Commands.Aulas
{
    public class AtivarAulaCommand : Command, IRequest<bool>
    {
        public Guid AulaId { get; set; }

        public override bool IsValid()
        {
            ValidationResult = new AtivarAulaCommandValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}

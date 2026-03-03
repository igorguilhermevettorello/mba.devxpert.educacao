using MediatR;
using PlataformaEducacional.Conteudo.Application.Validators;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Commands.Aulas
{
    public class AtualizarAulaCommand : Command, IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = default!;
        public string Descricao { get; set; } = default!;
        public int DuracaoMinutos { get; set; }
        public int Ordem { get; set; }

        public override bool IsValid()
        {
            ValidationResult = new AtualizarAulaCommandValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}

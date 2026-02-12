using FluentValidation.Results;
using MediatR;

namespace PlataformaEducacional.Core.Messages
{
    public abstract class Command : Message, IRequest<ValidationResult>
    {
        public DateTime Timestamp { get; private set; } = DateTime.Now;
        public ValidationResult ValidationResult { get; set; }

        public abstract bool IsValid();
    }
}

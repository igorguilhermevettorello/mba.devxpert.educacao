using FluentValidation.Results;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Core.Mediator
{
    public interface IMediatorHandler
    {
        Task PublishEvent<T>(T evento) where T : Event;
        Task<ValidationResult> SendCommand<T>(T comando) where T : Command;
    }
}

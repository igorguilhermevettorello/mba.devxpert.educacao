using MediatR;

namespace PlataformaEducacional.Alunos.Api.Application.Events;

public class AlunoEventHandler : INotificationHandler<AlunoRegistradoEvent>
{
    public Task Handle(AlunoRegistradoEvent notification, CancellationToken cancellationToken)
    {
        // Enviar evento de confirmação
        return Task.CompletedTask;
    }
}

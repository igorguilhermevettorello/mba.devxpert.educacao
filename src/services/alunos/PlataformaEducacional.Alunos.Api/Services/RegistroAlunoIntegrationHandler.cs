using FluentValidation.Results;
using PlataformaEducacional.Alunos.Api.Application.Commands;
using PlataformaEducacional.Core.Mediator;
using PlataformaEducacional.Core.Messages.Integration;
using PlataformaEducacional.MessageBus;

namespace PlataformaEducacional.Alunos.Api.Services;

public class RegistroAlunoIntegrationHandler : BackgroundService
{
    private readonly IMessageBus _bus;
    private readonly IServiceProvider _serviceProvider;

    public RegistroAlunoIntegrationHandler(IMessageBus bus, IServiceProvider serviceProvider)
    {
        _bus = bus;
        _serviceProvider = serviceProvider;
    }

    private void SetResponder()
    {
        _bus.RespondAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(async request =>
            await RegistrarAluno(request));

        _bus.AdvancedBus.Connected += OnConnect;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }

    private void OnConnect(object? s, EventArgs e)
    {
        SetResponder();
    }

    private async Task<ResponseMessage> RegistrarAluno(UsuarioRegistradoIntegrationEvent message)
    {
        var alunoCommand = new RegistrarAlunoCommand(message.Id, message.Nome, message.Email, message.Cpf);
        ValidationResult sucesso;

        using (var scope = _serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();
            sucesso = await mediator.SendCommand(alunoCommand);
        }

        return new ResponseMessage(sucesso);
    }
}

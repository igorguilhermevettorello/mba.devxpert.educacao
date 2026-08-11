using FluentValidation.Results;
using PlataformaEducacional.Alunos.Application.Commands;
using PlataformaEducacional.Core.Mediator;
using PlataformaEducacional.Core.Messages.Integration;
using PlataformaEducacional.MessageBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PlataformaEducacional.Alunos.Application.Services;

public class RegistroAlunoIntegrationHandler : BackgroundService
{
    private readonly IMessageBus _bus;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RegistroAlunoIntegrationHandler> _logger;

    public RegistroAlunoIntegrationHandler(IMessageBus bus, IServiceProvider serviceProvider, ILogger<RegistroAlunoIntegrationHandler> logger)
    {
        _bus = bus;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    private void SetResponder()
    {
        _bus.RespondAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(async request =>
            await RegistrarAluno(request));

        _bus.AdvancedBus.Connected += OnConnect;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Iniciando RegistroAlunoIntegrationHandler - Aguardando eventos de registro de usuário");
        SetResponder();
        return Task.CompletedTask;
    }

    private void OnConnect(object? s, EventArgs e)
    {
        _logger.LogInformation("MessageBus reconectado - Re-inscrevendo responders");
        SetResponder();
    }

    private async Task<ResponseMessage> RegistrarAluno(UsuarioRegistradoIntegrationEvent message)
    {
        _logger.LogInformation("Evento de registro de usuário recebido - UserId {UserId}, Email {Email}", message.Id, message.Email);

        var alunoCommand = new RegistrarAlunoCommand(message.Id, message.Nome, message.Email, message.Cpf);
        ValidationResult sucesso;

        using (var scope = _serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();
            sucesso = await mediator.SendCommand(alunoCommand);
        }

        if (sucesso.IsValid)
            _logger.LogInformation("Aluno registrado com sucesso - UserId {UserId}, Email {Email}", message.Id, message.Email);
        else
            _logger.LogWarning("Falha ao registrar aluno - UserId {UserId}: {Errors}", message.Id,
                string.Join(", ", sucesso.Errors.Select(e => e.ErrorMessage)));

        return new ResponseMessage(sucesso);
    }
}

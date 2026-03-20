using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlataformaEducacional.Alunos.Domain.Interfaces;
using PlataformaEducacional.Core.DomainObjects;
using PlataformaEducacional.Core.Messages.Integration;
using PlataformaEducacional.MessageBus;

namespace PlataformaEducacional.Alunos.Application.Services;

public class MatriculaIntegrationHandler : BackgroundService
{
    private readonly IMessageBus _bus;
    private readonly IServiceProvider _serviceProvider;

    public MatriculaIntegrationHandler(IMessageBus bus, IServiceProvider serviceProvider)
    {
        _bus = bus;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SetSubscribers();
        return Task.CompletedTask;
    }

    private void SetSubscribers()
    {
        _bus.SubscribeAsync<PedidoPagoIntegrationEvent>("MatriculaPaga",
            async request => await AtivarMatricula(request));

        _bus.SubscribeAsync<PedidoCanceladoIntegrationEvent>("MatriculaCancelada",
            async request => await CancelarMatricula(request));
    }

    private async Task AtivarMatricula(PedidoPagoIntegrationEvent message)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var alunoRepository = scope.ServiceProvider.GetRequiredService<IAlunoRepository>();

            var matricula = await alunoRepository.ObterMatriculaPorPedidoId(message.PedidoId);

            if (matricula == null)
            {
                throw new DomainException($"Matrícula não encontrada para o pedido {message.PedidoId}");
            }

            matricula.Ativar();
            alunoRepository.AtualizarMatricula(matricula);

            if (!await alunoRepository.UnitOfWork.Commit())
            {
                throw new DomainException($"Problemas ao ativar a matrícula do pedido {message.PedidoId}");
            }
        }
    }

    private async Task CancelarMatricula(PedidoCanceladoIntegrationEvent message)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var alunoRepository = scope.ServiceProvider.GetRequiredService<IAlunoRepository>();

            var matricula = await alunoRepository.ObterMatriculaPorPedidoId(message.PedidoId);

            if (matricula == null)
            {
                throw new DomainException($"Matrícula não encontrada para o pedido {message.PedidoId}");
            }

            matricula.Cancelar();
            alunoRepository.AtualizarMatricula(matricula);

            if (!await alunoRepository.UnitOfWork.Commit())
            {
                throw new DomainException($"Problemas ao cancelar a matrícula do pedido {message.PedidoId}");
            }
        }
    }
}

public static class RegistroMatriculaIntegrationHandler
{
    public static void AddMatriculaIntegrationHandler(this IServiceCollection services)
    {
        services.AddHostedService<MatriculaIntegrationHandler>();
    }
}
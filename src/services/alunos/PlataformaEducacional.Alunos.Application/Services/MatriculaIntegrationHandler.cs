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
        _bus.SubscribeAsync<PagamentoConfirmadoIntegrationEvent>("PagamentoConfirmado", async request => await AtivarMatricula(request));
    }

    private async Task AtivarMatricula(PagamentoConfirmadoIntegrationEvent message)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var alunoRepository = scope.ServiceProvider.GetRequiredService<IAlunoRepository>();

            var matricula = await alunoRepository.ObterMatriculaPorId(message.MatriculaId);

            if (matricula == null)
            {
                throw new DomainException($"Matrícula de id {message.MatriculaId} não encontrada");
            }

            matricula.Ativar();
            alunoRepository.AtualizarMatricula(matricula);

            if (!await alunoRepository.UnitOfWork.Commit())
            {
                throw new DomainException($"Problemas ao ativar a matrícula {message.MatriculaId}");
            }
        }
    }
}
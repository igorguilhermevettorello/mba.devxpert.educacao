using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlataformaEducacional.Alunos.Domain.Interfaces;
using PlataformaEducacional.Core.DomainObjects;
using PlataformaEducacional.Core.Messages.Integration;
using PlataformaEducacional.MessageBus;

namespace PlataformaEducacional.Alunos.Application.Services;

public class MatriculaIntegrationHandler : BackgroundService
{
    private readonly IMessageBus _bus;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MatriculaIntegrationHandler> _logger;

    public MatriculaIntegrationHandler(IMessageBus bus, IServiceProvider serviceProvider, ILogger<MatriculaIntegrationHandler> logger)
    {
        _bus = bus;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Iniciando MatriculaIntegrationHandler - Aguardando eventos de pagamento confirmado");
        SetSubscribers();
        return Task.CompletedTask;
    }

    private void SetSubscribers()
    {
        _bus.SubscribeAsync<PagamentoConfirmadoIntegrationEvent>("PagamentoConfirmado", async request => await AtivarMatricula(request));
    }

    private async Task AtivarMatricula(PagamentoConfirmadoIntegrationEvent message)
    {
        _logger.LogInformation("Evento de pagamento confirmado recebido - MatriculaId {MatriculaId}", message.MatriculaId);

        using (var scope = _serviceProvider.CreateScope())
        {
            var alunoRepository = scope.ServiceProvider.GetRequiredService<IAlunoRepository>();

            var matricula = await alunoRepository.ObterMatriculaPorId(message.MatriculaId);

            if (matricula == null)
            {
                _logger.LogError("Matrícula {MatriculaId} não encontrada ao tentar ativar", message.MatriculaId);
                throw new DomainException($"Matrícula de id {message.MatriculaId} não encontrada");
            }

            matricula.Ativar();
            alunoRepository.AtualizarMatricula(matricula);

            if (!await alunoRepository.UnitOfWork.Commit())
            {
                _logger.LogError("Falha ao persistir ativação da matrícula {MatriculaId}", message.MatriculaId);
                throw new DomainException($"Problemas ao ativar a matrícula {message.MatriculaId}");
            }

            _logger.LogInformation("Matrícula ativada com sucesso - MatriculaId {MatriculaId}", message.MatriculaId);
        }
    }
}
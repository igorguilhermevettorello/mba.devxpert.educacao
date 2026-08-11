using EasyNetQ;
using Microsoft.Extensions.Logging;
using PlataformaEducacional.Core.Messages.Integration;
using Polly;
using RabbitMQ.Client.Exceptions;

namespace PlataformaEducacional.MessageBus;

public class MessageBus : IMessageBus
{
    private IBus _bus;
    private IAdvancedBus _advancedBus;

    private readonly string _connectionString;
    private readonly ILogger<MessageBus> _logger;

    public MessageBus(string connectionString, ILogger<MessageBus> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
        TryConnect();
    }

    public bool IsConnected => _bus?.Advanced.IsConnected ?? false;
    public IAdvancedBus AdvancedBus => _bus?.Advanced;

    public void Publish<T>(T message) where T : IntegrationEvent
    {
        TryConnect();
        _bus.PubSub.Publish(message);
    }

    public async Task PublishAsync<T>(T message) where T : IntegrationEvent
    {
        TryConnect();
        await _bus.PubSub.PublishAsync(message);
    }

    public void Subscribe<T>(string subscriptionId, Action<T> onMessage) where T : class
    {
        TryConnect();
        _bus.PubSub.Subscribe(subscriptionId, onMessage);
    }

    public void SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage) where T : class
    {
        TryConnect();
        _bus.PubSub.SubscribeAsync(subscriptionId, onMessage);
    }

    public TResponse Request<TRequest, TResponse>(TRequest request) where TRequest : IntegrationEvent
        where TResponse : ResponseMessage
    {
        TryConnect();
        return _bus.Rpc.Request<TRequest, TResponse>(request);
    }

    public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request)
        where TRequest : IntegrationEvent where TResponse : ResponseMessage
    {
        TryConnect();
        
        // Adicionar timeout de 30 segundos
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        
        return await _bus.Rpc.RequestAsync<TRequest, TResponse>(request, 
            configure => configure.WithQueueName("PedidoIniciado"),
            cancellationTokenSource.Token);
    }

    public IDisposable Respond<TRequest, TResponse>(Func<TRequest, TResponse> responder)
        where TRequest : IntegrationEvent where TResponse : ResponseMessage
    {
        TryConnect();
        return _bus.Rpc.Respond(responder);
    }

    public async Task<IDisposable> RespondAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> responder)
        where TRequest : IntegrationEvent where TResponse : ResponseMessage
    {
        TryConnect();
        return await _bus.Rpc.RespondAsync(responder);
    }

    private void TryConnect()
    {
        if (IsConnected) return;

        _logger.LogInformation("Tentando conectar ao RabbitMQ com string: {ConnectionString}", _connectionString);

        var retryCount = 0;
        var policy = Policy.Handle<EasyNetQException>()
            .Or<BrokerUnreachableException>()
            .WaitAndRetry(3, retryAttempt =>
                {
                    retryCount++;
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                    _logger.LogWarning("Tentativa {RetryCount} de conexão ao RabbitMQ - aguardando {DelaySeconds} segundos", retryCount, delay.TotalSeconds);
                    return delay;
                },
                onRetry: (exception, timespan, retryAttempt, context) =>
                {
                    _logger.LogWarning(exception, "Falha na tentativa {RetryCount} ao conectar ao RabbitMQ", retryAttempt);
                });

        try
        {
            policy.Execute(() =>
            {
                _bus = RabbitHutch.CreateBus(_connectionString, s => s.EnableSystemTextJson());
                _advancedBus = _bus.Advanced;
                _advancedBus.Disconnected += OnDisconnect;
                _logger.LogInformation("Conectado ao RabbitMQ com sucesso");
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao conectar ao RabbitMQ após todas as tentativas");
            throw;
        }
    }

    private void OnDisconnect(object? s, DisconnectedEventArgs e)
    {
        _logger.LogWarning("Desconectado do RabbitMQ - tentando reconectar");
        
        var policy = Policy.Handle<EasyNetQException>()
            .Or<BrokerUnreachableException>()
            .RetryForever();

        policy.Execute(TryConnect);
    }

    public void Dispose()
    {
        _bus.Dispose();
    }
}

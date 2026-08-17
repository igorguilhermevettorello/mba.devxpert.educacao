using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace PlataformaEducacional.WebApi.Core.Configurations;

public class PollyConfig
{
    private readonly ILogger<PollyConfig> _logger;

    public PollyConfig(ILogger<PollyConfig> logger)
    {
        _logger = logger;
    }

    public IHttpClientBuilder AddRetryAndCircuitBreaker(IHttpClientBuilder builder)
    {
        builder.AddPolicyHandler(EsperarTentar())
               .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

        return builder;
    }

    private AsyncRetryPolicy<HttpResponseMessage> EsperarTentar()
    {
        var sleepsBeetweenRetries = new[]
        {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(10)
        };

        var retry = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(sleepsBeetweenRetries, onRetry: (_, span, retryCount, _) =>
            {
                _logger.LogWarning("Retentativa {RetryCount} de requisição HTTP - Tempo de Espera: {DelaySeconds} segundos",
                    retryCount, span.TotalSeconds);
            });

        return retry;
    }
}

public static class PollyConfigExtensions
{
    public static IHttpClientBuilder AddRetryAndCircuitBreaker(this IHttpClientBuilder builder)
    {
        var services = builder.Services;

        services.AddSingleton<PollyConfig>();

        var serviceProvider = services.BuildServiceProvider();
        var pollyConfig = serviceProvider.GetRequiredService<PollyConfig>();

        return pollyConfig.AddRetryAndCircuitBreaker(builder);
    }
}

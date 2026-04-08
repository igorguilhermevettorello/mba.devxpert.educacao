using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace PlataformaEducacional.WebApi.Core.Configurations;

public static class PollyConfig
{
    public static IHttpClientBuilder AddRetryAndCircuitBreaker(this IHttpClientBuilder builder)
    {
        //builder.AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(500)));

        builder.AddPolicyHandler(EsperarTentar())
               .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

        return builder;
    }

    private static AsyncRetryPolicy<HttpResponseMessage> EsperarTentar()
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
                var previousBackgroundColor = Console.BackgroundColor;
                var previousForegroundColor = Console.ForegroundColor;

                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;

                Console.Out.WriteLineAsync($" ***** {DateTime.Now:HH:mm:ss} | " +
                    $"Retentativa: {retryCount} | " +
                    $"Tempo de Espera em segundos: {span.TotalSeconds} **** ");

                Console.BackgroundColor = previousBackgroundColor;
                Console.ForegroundColor = previousForegroundColor;
            });

        return retry;
    }
    
}

using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace PlataformaEducacional.WebApi.Core.Configurations;

public static class PollyConfig
{
    public static IHttpClientBuilder AddRetryAndCircuitBreaker(this IHttpClientBuilder builder)
    {
        builder.AddPolicyHandler(EsperarTentar())
               .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

        return builder;
    }

    private static AsyncRetryPolicy<HttpResponseMessage> EsperarTentar()
    {
        var retry = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(new[]
            {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
            });

        return retry;
    }
        
}

using PlataformaEducacional.Bff.Api.Extensions;
using PlataformaEducacional.Bff.Api.Interfaces;
using PlataformaEducacional.Bff.Api.Services;
using PlataformaEducacional.WebApi.Core.Configurations;
using PlataformaEducacional.WebApi.Core.User;

namespace PlataformaEducacional.Bff.Api.Configurations;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {

        var alunoUrl = configuration.GetValue<string>("AlunoApiUrl");
        var conteudoUrl = configuration.GetValue<string>("ConteudoApiUrl");
        var pagamentoUrl = configuration.GetValue<string>("PagamentoApiUrl");

        services.Configure<AppServicesSettings>(configuration);

        if (string.IsNullOrEmpty(alunoUrl))
        {
            throw new InvalidOperationException("AlunoApiUrl is not configured properly.");
        }

        if (string.IsNullOrEmpty(conteudoUrl))
        {
            throw new InvalidOperationException("ConteudoApiUrl is not configured properly.");
        }

        if (string.IsNullOrEmpty(pagamentoUrl))
        {
            throw new InvalidOperationException("PagamentoApiUrl is not configured properly.");
        }

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IAspNetUser, AspNetUser>();
        services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

        services.AddScoped<IMatriculaService, MatriculaService>();
        services.AddScoped<IConteudoService, ConteudoService>();
        services.AddScoped<IPagamentoService, PagamentoService>();

        services.AddHttpClient<IMatriculaService, MatriculaService>(client =>
        {
            client.BaseAddress = new Uri(alunoUrl); // URL da Matricula API
        })
        .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
        .AddRetryAndCircuitBreaker();

        services.AddHttpClient<IConteudoService, ConteudoService>(client =>
        {
            client.BaseAddress = new Uri(conteudoUrl); // URL da Conteudo API
        })
        .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
        .AddRetryAndCircuitBreaker();

        services.AddHttpClient<IPagamentoService, PagamentoService>(client =>
        {
            client.BaseAddress = new Uri(pagamentoUrl); // URL da Pagamento API
        }) .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
        .AddRetryAndCircuitBreaker();
    }
}

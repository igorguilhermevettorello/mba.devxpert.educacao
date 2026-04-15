using PlataformaEducacional.Core.Mediator;
using PlataformaEducacional.Core.Notifications;
using PlataformaEducacional.Pagamentos.Api.Data;
using PlataformaEducacional.Pagamentos.Api.Data.Repository;
using PlataformaEducacional.Pagamentos.Api.Facade;
using PlataformaEducacional.Pagamentos.Api.Models;
using PlataformaEducacional.Pagamentos.Api.Services;
using PlataformaEducacional.WebApi.Core.User;

using PlataformaEducacional.WebApi.Core.Configurations;

namespace PlataformaEducacional.Pagamentos.Api.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<INotificador, Notificador>();
        services.AddScoped<IAspNetUser, AspNetUser>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IMediatorHandler, MediatorHandler>();
        services.AddScoped<IPagamentoService, PagamentoService>();
        services.AddScoped<IPagamentoRepository, PagamentoRepository>();
        services.AddScoped<IPagamentoFacade, PagamentoCartaoCreditoFacade>();
        services.AddScoped<PagamentosContext>();

        services.AddHttpClient<IConteudoService, ConteudoService>(client =>
        {
            client.BaseAddress = new Uri(configuration["ConteudoUrl"]);
        })
        .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
        .AddRetryAndCircuitBreaker();

        services.AddHttpClient<IAlunoService, AlunoService>(client =>
        {
            client.BaseAddress = new Uri(configuration["AlunoUrl"]);
        })
        .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
        .AddRetryAndCircuitBreaker();

    }
}

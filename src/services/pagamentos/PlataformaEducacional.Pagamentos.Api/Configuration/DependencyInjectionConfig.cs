using PlataformaEducacional.Pagamentos.Api.Data.Repository;
using PlataformaEducacional.Pagamentos.Api.Data;
using PlataformaEducacional.Pagamentos.Api.Facade;
using PlataformaEducacional.Pagamentos.Api.Services;
using PlataformaEducacional.Pagamentos.Api.Models;

namespace PlataformaEducacional.Pagamentos.Api.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IPagamentoService, PagamentoService>();
        services.AddScoped<IPagamentoRepository, PagamentoRepository>();
        services.AddScoped<IPagamentoFacade, PagamentoCartaoCreditoFacade>();
        services.AddScoped<PagamentosContext>();

        // IMPORTANTE: Registrar o handler de integração
        services.AddHostedService<PagamentoIntegrationHandler>();
    }
}

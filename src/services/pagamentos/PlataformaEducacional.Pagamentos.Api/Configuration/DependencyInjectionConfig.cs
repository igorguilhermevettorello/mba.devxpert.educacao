using PlataformaEducacional.Core.Mediator;
using PlataformaEducacional.Pagamentos.Api.Data;
using PlataformaEducacional.Pagamentos.Api.Data.Repository;
using PlataformaEducacional.Pagamentos.Api.Facade;
using PlataformaEducacional.Pagamentos.Api.Models;
using PlataformaEducacional.Pagamentos.Api.Services;

namespace PlataformaEducacional.Pagamentos.Api.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IMediatorHandler, MediatorHandler>();
        services.AddScoped<IPagamentoService, PagamentoService>();
        services.AddScoped<IPagamentoRepository, PagamentoRepository>();
        services.AddScoped<IPagamentoFacade, PagamentoCartaoCreditoFacade>();
        services.AddScoped<PagamentosContext>();

        // IMPORTANTE: Registrar o handler de integração
        //TODO: avaliar necessidade do codigo abaixo
        //services.AddHostedService<PagamentoIntegrationHandler>();
    }
}

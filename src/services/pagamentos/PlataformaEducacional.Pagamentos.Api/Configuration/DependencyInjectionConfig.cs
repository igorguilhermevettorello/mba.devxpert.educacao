using PlataformaEducacional.Core.Mediator;
using PlataformaEducacional.Core.Notifications;
using PlataformaEducacional.Pagamentos.Api.Data;
using PlataformaEducacional.Pagamentos.Api.Data.Repository;
using PlataformaEducacional.Pagamentos.Api.Facade;
using PlataformaEducacional.Pagamentos.Api.Models;
using PlataformaEducacional.Pagamentos.Api.Services;
using PlataformaEducacional.WebApi.Core.User;

namespace PlataformaEducacional.Pagamentos.Api.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<INotificador, Notificador>();
        services.AddScoped<IAspNetUser, AspNetUser>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IMediatorHandler, MediatorHandler>();
        services.AddScoped<IPagamentoService, PagamentoService>();
        services.AddScoped<IPagamentoRepository, PagamentoRepository>();
        services.AddScoped<IPagamentoFacade, PagamentoCartaoCreditoFacade>();
        services.AddScoped<PagamentosContext>();
    }
}

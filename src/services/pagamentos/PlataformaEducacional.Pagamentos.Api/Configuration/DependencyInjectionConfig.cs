using PlataformaEducacional.Pagamentos.Api.Data;
using PlataformaEducacional.Pagamentos.Api.Data.Repository;
using PlataformaEducacional.Pagamentos.Api.Facade;
using PlataformaEducacional.Pagamentos.Api.Models;
using PlataformaEducacional.Pagamentos.Api.Services;
using PlataformaEducacional.WebApi.Core.User;

namespace PlataformaEducacional.Pagamentos.Api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();

            services.AddScoped<IPagamentoService, PagamentoService>();
            services.AddScoped<IPagamentoFacade, PagamentoCartaoCreditoFacade>();

            services.AddScoped<IPagamentoRepository, PagamentoRepository>();
            services.AddScoped<PagamentosContext>();
        }
    }
}

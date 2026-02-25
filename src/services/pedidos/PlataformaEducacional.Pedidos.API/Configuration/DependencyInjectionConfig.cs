using NetDevPack.SimpleMediator.Core.Interfaces;
using PlataformaEducacional.Core.Mediator;
using PlataformaEducacional.Pedidos.API.Application.Commands;
using PlataformaEducacional.Pedidos.API.Application.Events;
using PlataformaEducacional.Pedidos.API.Application.Queries;
using PlataformaEducacional.Pedidos.Domain.Pedidos;
using PlataformaEducacional.Pedidos.Domain.Vouchers;
using PlataformaEducacional.Pedidos.Infra.Data;
using PlataformaEducacional.Pedidos.Infra.Data.Repository;
using PlataformaEducacional.WebApi.Core.User;

namespace PlataformaEducacional.Pedidos.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            // API
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();

            // Commands
            services.AddScoped<IRequestHandler<AdicionarPedidoCommand, ValidationResult>, PedidoCommandHandler>();

            // Events
            services.AddScoped<INotificationHandler<PedidoRealizadoEvent>, PedidoEventHandler>();

            // Application
            services.AddScoped<IMediatorHandler, MediatorHandler>();
            services.AddScoped<IVoucherQueries, VoucherQueries>();
            services.AddScoped<IPedidoQueries, PedidoQueries>();

            // Data
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<PedidosContext>();
        }
    }
}

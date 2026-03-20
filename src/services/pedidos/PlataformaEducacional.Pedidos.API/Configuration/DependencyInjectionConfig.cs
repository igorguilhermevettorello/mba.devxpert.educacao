using FluentValidation.Results;
using MediatR;
using PlataformaEducacional.Core.Mediator;
using PlataformaEducacional.Pedidos.API.Application.Commands;
using PlataformaEducacional.Pedidos.API.Application.Events;
using PlataformaEducacional.Pedidos.API.Application.Queries;
using PlataformaEducacional.Pedidos.Data;
using PlataformaEducacional.Pedidos.Data.Repository;
using PlataformaEducacional.Pedidos.Domain.Pedidos;
using PlataformaEducacional.Pedidos.Domain.Vouchers;
using PlataformaEducacional.WebApi.Core.User;

namespace PlataformaEducacional.Pedidos.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            // Mediator
            services.AddScoped<IMediatorHandler, MediatorHandler>();
            
            // Commands
            services.AddScoped<IRequestHandler<AdicionarPedidoCommand, ValidationResult>, PedidoCommandHandler>();
            services.AddScoped<IRequestHandler<AdicionarVoucherCommand, ValidationResult>, VoucherCommandHandler>();

            // Events
            services.AddScoped<INotificationHandler<PedidoRealizadoEvent>, PedidoEventHandler>();

            // Queries
            services.AddScoped<IPedidoQueries, PedidoQueries>();
            services.AddScoped<IVoucherQueries, VoucherQueries>();

            // Http Context
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();

            // Repositories
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            
            // Context
            services.AddScoped<PedidosContext>();
        }
    }
}

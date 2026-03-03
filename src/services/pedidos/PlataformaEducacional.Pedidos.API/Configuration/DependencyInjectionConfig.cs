using FluentValidation.Results;
using MediatR;
using PlataformaEducacional.Core.Mediator;
using PlataformaEducacional.Pedidos.API.Application.Commands;
using PlataformaEducacional.Pedidos.API.Application.Events;
using PlataformaEducacional.Pedidos.Data;
using PlataformaEducacional.Pedidos.Data.Repository;
using PlataformaEducacional.Pedidos.Domain.Pedidos;
using PlataformaEducacional.Pedidos.Domain.Vouchers;

namespace PlataformaEducacional.Pedidos.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IMediatorHandler, MediatorHandler>();            
            services.AddScoped<IRequestHandler<AdicionarPedidoCommand, ValidationResult>, PedidoCommandHandler>();
            
            services.AddScoped<INotificationHandler<PedidoRealizadoEvent>, PedidoEventHandler>();

            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<PedidosContext>();
        }
    }
}

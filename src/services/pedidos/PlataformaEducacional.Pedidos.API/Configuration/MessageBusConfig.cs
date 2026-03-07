using PlataformaEducacional.MessageBus;
using PlataformaEducacional.Core.Extensions;
using PlataformaEducacional.Pedidos.API.Services;

namespace PlataformaEducacional.Pedidos.API.Configuration
{
    public static class MessageBusConfig
    {
        public static void AddMessageBusConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"))
                .AddHostedService<PedidoOrquestradorIntegrationHandler>();
        }
    }
}

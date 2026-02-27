using PlataformaEducacional.MessageBus;
using PlataformaEducacional.Core.Extensions;

namespace PlataformaEducacional.Pedidos.API.Configuration
{
    public static class MessageBusConfig
    {
        public static void AddMessageBusConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"));
        }
    }
}

using PlataformaEducacional.Core.Extensions;
using PlataformaEducacional.MessageBus;

namespace PlataformaEducacional.Pagamentos.Api.Configuration
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

using PlataformaEducacional.MessageBus;
using PlataformaEducacional.Pagamentos.Api.Services;
using PlataformaEducacional.Core.Extensions;

namespace PlataformaEducacional.Pagamentos.Api.Configuration
{
    public static class MessageBusConfig
    {
        public static void AddMessageBusConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"))
                .AddHostedService<PagamentoIntegrationHandler>();
        }
    }
}

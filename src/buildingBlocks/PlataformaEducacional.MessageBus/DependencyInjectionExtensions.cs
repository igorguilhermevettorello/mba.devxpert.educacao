using Microsoft.Extensions.DependencyInjection;

namespace PlataformaEducacional.MessageBus;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services, string connection)
    {
        if (string.IsNullOrEmpty(connection)) throw new ArgumentNullException();

        services.AddSingleton<IMessageBus>(serviceProvider =>
        {
            var logger = serviceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<MessageBus>>();
            return new MessageBus(connection, logger);
        });

        return services;
    }
}

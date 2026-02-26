using PlataformaEducacional.Conteudo.Data.Repositories;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Core.Mediator;
using PlataformaEducacional.Core.Notifications;

namespace PlataformaEducacional.Conteudo.Api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<INotificador, Notificador>();
            services.AddScoped<IMediatorHandler, MediatorHandler>();
            services.AddScoped<ICursoRepository, CursoRepository>();
            services.AddScoped<IAulaRepository, AulaRepository>();
            //services.AddScoped<IRequestHandler<RegistrarAlunoCommand, ValidationResult>, AlunoCommandHandler>();

            //services.AddScoped<INotificationHandler<AlunoRegistradoEvent>, AlunoEventHandler>();

            //services.AddScoped<IAlunoRepository, AlunoRepository>();
        }
    }
}

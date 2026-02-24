using FluentValidation.Results;
using MediatR;
using PlataformaEducacional.Alunos.Api.Application.Commands;
using PlataformaEducacional.Alunos.Api.Application.Events;
using PlataformaEducacional.Alunos.Api.Data;
using PlataformaEducacional.Alunos.Api.Data.Repository;
using PlataformaEducacional.Alunos.Api.Interfaces;
using PlataformaEducacional.Core.Mediator;

namespace PlataformaEducacional.Alunos.Api.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IMediatorHandler, MediatorHandler>();
        services.AddScoped<IRequestHandler<RegistrarAlunoCommand, ValidationResult>, AlunoCommandHandler>();

        services.AddScoped<INotificationHandler<AlunoRegistradoEvent>, AlunoEventHandler>();

        services.AddScoped<IAlunoRepository, AlunoRepository>();
        services.AddScoped<AlunosContext>();
    }
}

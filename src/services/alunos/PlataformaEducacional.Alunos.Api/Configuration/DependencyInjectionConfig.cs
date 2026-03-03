using FluentValidation.Results;
using MediatR;
using PlataformaEducacional.Alunos.Application.Commands;
using PlataformaEducacional.Alunos.Application.Events;
using PlataformaEducacional.Alunos.Data;
using PlataformaEducacional.Alunos.Data.Repository;
using PlataformaEducacional.Alunos.Domain.Interfaces;
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

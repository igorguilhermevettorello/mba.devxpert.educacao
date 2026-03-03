using FluentValidation.Results;
using MediatR;
using PlataformaEducacional.Alunos.Application.Commands;
using PlataformaEducacional.Alunos.Application.Events;
using PlataformaEducacional.Alunos.Data;
using PlataformaEducacional.Alunos.Data.Repository;
using PlataformaEducacional.Alunos.Domain.Interfaces;
using PlataformaEducacional.Core.Mediator;
using PlataformaEducacional.WebApi.Core.User;

namespace PlataformaEducacional.Alunos.Api.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IMediatorHandler, MediatorHandler>();
        services.AddScoped<IRequestHandler<RegistrarAlunoCommand, ValidationResult>, AlunoCommandHandler>();
        services.AddScoped<IRequestHandler<RealizarMatriculaCommand, ValidationResult>, AlunoCommandHandler>();
        services.AddScoped<IRequestHandler<AdicionarEnderecoCommand, ValidationResult>, AlunoCommandHandler>();
        services.AddScoped<IRequestHandler<RegistrarProgressoCommand, ValidationResult>, AlunoCommandHandler>();
        services.AddScoped<IRequestHandler<EmitirCertificadoCommand, ValidationResult>, AlunoCommandHandler>();

        services.AddScoped<INotificationHandler<AlunoRegistradoEvent>, AlunoEventHandler>();

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IAspNetUser, AspNetUser>();

        services.AddScoped<IAlunoRepository, AlunoRepository>();
        services.AddScoped<AlunosContext>();
    }
}

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using PlataformaEducacional.Conteudo.Api.Configuration;
using PlataformaEducacional.Conteudo.Api.Configuration.Seed;
using PlataformaEducacional.Conteudo.Api.HealthChecks;
using PlataformaEducacional.WebApi.Core.Configurations;
using PlataformaEducacional.WebApi.Core.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Configuration.AddUserSecrets<Program>(optional: true, reloadOnChange: true);

if (builder.Environment.IsEnvironment("Docker"))
{
    var dockerSecretsPath = Environment.GetEnvironmentVariable("DOCKER_SECRETS_PATH");

    if (!string.IsNullOrWhiteSpace(dockerSecretsPath))
    {
        builder.Configuration.AddJsonFile(dockerSecretsPath, optional: true, reloadOnChange: false);
    }
}

builder.AddDataContextConfiguration();
builder.Services.AddAutoMapper(cfg => { }, typeof(Program));

// Add services to the container.
builder.Services.AddApiConfiguration("Conteudo API");

// Adicionar configuração JWT
builder.Services.AddJwtConfiguration(builder.Configuration, builder.Environment);
builder.Services.AddMediatR(cfg =>
{
    cfg.LicenseKey = builder.Configuration.GetValue<string>("mediator-license") ?? string.Empty;
    cfg.RegisterServicesFromAssemblies(typeof(PlataformaEducacional.Conteudo.Application.Commands.Cursos.CriarCursoCommand).Assembly,
                                       typeof(PlataformaEducacional.Conteudo.Application.Commands.Cursos.AtualizarCursoCommand).Assembly);
});

builder.Services.RegisterServices();
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database", tags: ["ready"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseApiConfiguration(app.Environment);

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.UseDatabaseMigrationStartData();

app.Run();

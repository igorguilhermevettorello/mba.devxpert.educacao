using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using PlataformaEducacional.Alunos.Api.Configuration;
using PlataformaEducacional.Alunos.Api.Configuration.Seed;
using PlataformaEducacional.Alunos.Api.HealthChecks;
using PlataformaEducacional.WebApi.Core.Configurations;
using PlataformaEducacional.WebApi.Core.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.AddDataContextConfiguration();
builder.Services.AddApiConfiguration("Alunos API");
builder.Services.AddJwtConfiguration(builder.Configuration, builder.Environment);

builder.Services.AddMediatR(cfg =>
{
    cfg.LicenseKey = builder.Configuration.GetValue<string>("mediator-license") ?? string.Empty;
    cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
});

builder.Services.RegisterServices(builder.Configuration);

builder.Services.AddMessageBusConfiguration(builder.Configuration);
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

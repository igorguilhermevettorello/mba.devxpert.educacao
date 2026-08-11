using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using PlataformaEducacional.Auth.Api.Configurations;
using PlataformaEducacional.Auth.Api.Configurations.Seed;
using PlataformaEducacional.Auth.Api.HealthChecks;
using PlataformaEducacional.Auth.Api.Security;
using PlataformaEducacional.WebApi.Core.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddUserSecrets<Program>(optional: true, reloadOnChange: true);
builder.AddDataContextConfiguration();
builder.Services.AddApiConfiguration("Auth API", authenticationRequired: false, setProxyReverse: builder.Environment.IsEnvironment("Docker"));
builder.Services.AddIdentityConfiguration(builder.Configuration, builder.Environment);
builder.Services.AddSingleton<IJwtRsaSigningCredentialsProvider, JwtRsaSigningCredentialsProvider>();

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

app.UseDatabseMigrationStartData();
app.Run();

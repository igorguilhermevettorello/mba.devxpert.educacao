using PlataformaEducacional.Alunos.Api.Configuration;
using PlataformaEducacional.Alunos.Api.Configuration.Seed;
using PlataformaEducacional.WebApi.Core.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.AddDataContextConfiguration();
builder.Services.AddApiConfiguration("Alunos API");

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

builder.Services.RegisterServices();

builder.Services.AddMessageBusConfiguration(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseApiConfiguration(app.Environment);

app.UseDatabaseMigrationStartData();

app.Run();

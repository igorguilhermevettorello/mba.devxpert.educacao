using PlataformaEducacional.Conteudo.Api.Configuration;
using PlataformaEducacional.Conteudo.Api.Configuration.Seed;
using PlataformaEducacional.WebApi.Core.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.AddDataContextConfiguration();

// Add services to the container.
builder.Services.AddApiConfiguration("Contedo API");

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PlataformaEducacional.Conteudo.Application.Commands.Cursos.CriarCursoCommand).Assembly));

builder.Services.RegisterServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseApiConfiguration(app.Environment);

app.UseDatabaseMigrationStartData();

app.Run();

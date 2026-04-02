using PlataformaEducacional.Conteudo.Api.Configuration;
using PlataformaEducacional.Conteudo.Api.Configuration.Seed;
using PlataformaEducacional.WebApi.Core.Configurations;
using PlataformaEducacional.WebApi.Core.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.AddDataContextConfiguration();

builder.Services.AddAutoMapper(cfg => { }, typeof(Program));

// Add services to the container.
builder.Services.AddApiConfiguration("Conteudo API");

// Adicionar configuração JWT
builder.Services.AddJwtConfiguration(builder.Configuration, builder.Environment);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PlataformaEducacional.Conteudo.Application.Commands.Cursos.CriarCursoCommand).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PlataformaEducacional.Conteudo.Application.Commands.Cursos.AtualizarCursoCommand).Assembly));

builder.Services.RegisterServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseApiConfiguration(app.Environment);

app.UseDatabaseMigrationStartData();

app.Run();

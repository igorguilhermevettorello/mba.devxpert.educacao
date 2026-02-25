using PlataformaEducacional.WebApi.Core.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiConfiguration("Pedidos API");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseApiConfiguration(app.Environment);

app.Run();

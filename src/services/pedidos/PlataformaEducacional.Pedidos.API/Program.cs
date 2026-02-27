using PlataformaEducacional.Pedidos.API.Configuration;
using PlataformaEducacional.WebApi.Core.Configurations;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.AddDataContextConfiguration();

builder.Services.AddApiConfiguration("Pedidos API");

//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.Load("PlataformaEducacional.Pedidos.Domain")));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

builder.Services.RegisterServices();

builder.Services.AddMessageBusConfiguration(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseApiConfiguration(app.Environment);

app.UseDatabaseMigrationStartData();

app.Run();

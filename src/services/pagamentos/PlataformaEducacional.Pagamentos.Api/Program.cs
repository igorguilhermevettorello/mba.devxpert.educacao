using PlataformaEducacional.Pagamentos.Api.Configuration;
using PlataformaEducacional.Pagamentos.Api.Configuration.Seed;
using PlataformaEducacional.Pagamentos.Api.Facade;
using PlataformaEducacional.WebApi.Core.Configurations;
using PlataformaEducacional.WebApi.Core.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.AddDataContextConfiguration();
builder.Services.AddAutoMapper(cfg => { }, typeof(Program));
builder.Services.AddApiConfiguration("Pagamentos API");
builder.Services.Configure<PagamentoConfig>(builder.Configuration.GetSection("PagamentoConfig"));
builder.Services.AddJwtConfiguration(builder.Configuration, builder.Environment);

builder.Services.AddMediatR(cfg =>
{
    cfg.LicenseKey = builder.Configuration.GetValue<string>("mediator-license") ?? string.Empty;
    cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
});

builder.Services.RegisterServices(builder.Configuration);
builder.Services.AddMessageBusConfiguration(builder.Configuration);

var app = builder.Build();
app.UseApiConfiguration(app.Environment);
app.UseDatabaseMigrationStartData();
app.Run();

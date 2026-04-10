using PlataformaEducacional.Bff.Api.Configurations;
using PlataformaEducacional.WebApi.Core.Configurations;
using PlataformaEducacional.WebApi.Core.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApiConfiguration("BFF API");
builder.Services.AddJwtConfiguration(builder.Configuration);

builder.Services.AddMediatR(cfg =>
{
    cfg.LicenseKey = builder.Configuration.GetValue<string>("mediator-license") ?? string.Empty;
    cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
});

builder.Services.RegisterServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseApiConfiguration(app.Environment);

app.Run();

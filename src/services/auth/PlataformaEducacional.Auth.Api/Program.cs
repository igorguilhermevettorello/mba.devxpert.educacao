using PlataformaEducacional.Auth.Api.Configurations;
using PlataformaEducacional.Auth.Api.Configurations.Seed;
using PlataformaEducacional.WebApi.Core.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddDataContextConfiguration();
builder.Services.AddApiConfiguration("Auth API", authenticationRequired: false);
builder.Services.AddIdentityConfiguration(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseApiConfiguration(app.Environment);

app.UseDatabseMigrationStartData();
app.Run();

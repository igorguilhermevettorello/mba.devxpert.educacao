using PlataformaEducacional.WebApi.Core.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.AddSwagger("Pagamentos API");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UsarSwagger();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

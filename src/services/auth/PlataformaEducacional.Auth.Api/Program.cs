using PlataformaEducacional.Auth.Api.Configurations;
using PlataformaEducacional.WebApi.Core.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddDataContextConfiguration();
builder.Services.AddApiConfiguration("Auth API");
builder.Services.AddIdentityConfiguration(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseApiConfiguration(app.Environment);

//if (app.Environment.IsDevelopment())
//{
//    app.UsarSwagger();
//}

//app.UseHttpsRedirection();

//app.UseAuthConfiguration();

//app.MapControllers();

app.Run();

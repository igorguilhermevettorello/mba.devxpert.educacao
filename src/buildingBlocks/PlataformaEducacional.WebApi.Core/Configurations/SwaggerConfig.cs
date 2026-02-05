using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace PlataformaEducacional.WebApi.Core.Configurations
{
    public static class SwaggerConfig
    {
        public static void AddSwagger(this WebApplicationBuilder builder, string apiName)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "Plataforma Educacional Enterprise",
                    Description = $"<b><i>{apiName}</i></b> faz parte do Plataforma Educacional Enterprise Applications.",
                    Contact = new OpenApiContact() 
                    { 
                        Name = "Grupo 5 | MBA DevIO", Email = "grupo5@mbaDev.io" 
                    },
                    License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
                });
            });
        }

        public static void UsarSwagger(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });
        }
    }
}

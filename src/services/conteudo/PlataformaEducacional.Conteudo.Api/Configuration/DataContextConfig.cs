using PlataformaEducacional.Conteudo.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace PlataformaEducacional.Conteudo.Api.Configuration
{
    public static class DataContextConfig
    {
        public static void AddDataContextConfiguration(this WebApplicationBuilder builder)
        {
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddDbContextPool<CursoContext>(options =>
                {
                    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnectionLite"))
                           .EnableDetailedErrors()
                           .EnableSensitiveDataLogging();
                });
            }
            else
            {
                builder.Services.AddDbContext<CursoContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            }

            builder.Services.AddScoped<CursoContext>();
        }
    }
}

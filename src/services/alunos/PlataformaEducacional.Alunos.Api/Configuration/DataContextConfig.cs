using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Alunos.Api.Data;

namespace PlataformaEducacional.Alunos.Api.Configuration
{
    public static class DataContextConfig
    {
        public static void AddDataContextConfiguration(this WebApplicationBuilder builder)
        {
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddDbContextPool<AlunosContext>(options =>
                {
                    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnectionLite"))
                           .EnableDetailedErrors()
                           .EnableSensitiveDataLogging();
                });
            }
            else
            {
                builder.Services.AddDbContext<AlunosContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            }

            builder.Services.AddScoped<AlunosContext>();
        }
    }
}

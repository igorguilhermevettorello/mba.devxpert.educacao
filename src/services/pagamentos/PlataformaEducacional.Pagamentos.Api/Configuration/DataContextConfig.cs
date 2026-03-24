using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Pagamentos.Api.Data;

namespace PlataformaEducacional.Pagamentos.Api.Configuration
{
    public static class DataContextConfig
    {
        public static void AddDataContextConfiguration(this WebApplicationBuilder builder)
        {
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddDbContextPool<PagamentosContext>(options =>
                {
                    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnectionLite"))
                           .EnableDetailedErrors()
                           .EnableSensitiveDataLogging();
                });
            }
            else
            {
                builder.Services.AddDbContext<PagamentosContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            }

            builder.Services.AddScoped<PagamentosContext>();
        }
    }
}

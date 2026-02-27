using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Pedidos.Data;

namespace PlataformaEducacional.Pedidos.API.Configuration
{
    public static class DataContextConfig
    {
        public static void AddDataContextConfiguration(this WebApplicationBuilder builder)
        {
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddDbContextPool<PedidosContext>(options =>
                {
                    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnectionLite"))
                           .EnableDetailedErrors()
                           .EnableSensitiveDataLogging();
                });
            }
            else
            {
                builder.Services.AddDbContext<PedidosContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            }

            builder.Services.AddScoped<PedidosContext>();
        }
    }
}

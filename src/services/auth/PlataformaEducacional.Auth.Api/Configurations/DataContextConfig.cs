using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Auth.Api.Data;

namespace PlataformaEducacional.Auth.Api.Configurations
{
    public static class DataContextConfig
    {
        public static void AddDataContextConfiguration(this WebApplicationBuilder builder)
        {
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
                {
                    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnectionLite"))
                           .EnableDetailedErrors()
                           .EnableSensitiveDataLogging();
                });
            }       
            else
            {
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            }

            builder.Services.AddScoped<ApplicationDbContext>();
        }
    }
}

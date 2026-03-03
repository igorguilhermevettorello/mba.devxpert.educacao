using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Conteudo.Data.Context;

namespace PlataformaEducacional.Conteudo.Api.Configuration.Seed
{
    public static class DatabaseMigrationStartDataExtension
    {
        public static void UseDatabaseMigrationStartData(this WebApplication app)
        {
            EnsureSeedData(app).Wait();
        }

        private static async Task EnsureSeedData(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var env = services.GetRequiredService<IWebHostEnvironment>();

            try
            {
                if (env.IsDevelopment() || env.IsEnvironment("Docker") || env.IsStaging())
                {
                    var context = services.GetRequiredService<CursoContext>();

                    await context.Database.MigrateAsync();

                    // await EnsureSeedAlunos(context);
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating the database.");
            }
        }
    }
}

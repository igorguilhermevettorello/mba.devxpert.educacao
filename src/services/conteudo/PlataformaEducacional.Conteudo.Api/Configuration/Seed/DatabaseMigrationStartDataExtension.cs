using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Conteudo.Data.Context;
using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Conteudo.Domain.Enums;

namespace PlataformaEducacional.Conteudo.Api.Configuration.Seed
{
    public static class DatabaseMigrationStartDataExtension
    {
        public static void UseDatabaseMigrationStartData(this WebApplication app)
        {
            EnsureSeedData(app);
        }

        private static void EnsureSeedData(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var env = services.GetRequiredService<IWebHostEnvironment>();

            try
            {
                if (env.IsDevelopment() || env.IsEnvironment("Docker") || env.IsStaging())
                {
                    var context = services.GetRequiredService<CursoContext>();
                    context.Database.Migrate();
                    EnsureSeedCursos(context);
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating the database.");
            }
        }

        private static void EnsureSeedCursos(CursoContext context)
        {
            if (context.Cursos.Any())
                return;

            var curso = new Curso("Curso Mockado", "Curso criado no seed de conteúdo", "Vaniel Dorcaro", NivelCurso.Avancado, 2500);
            context.Add(curso);
            context.SaveChanges();
        }
    }
}

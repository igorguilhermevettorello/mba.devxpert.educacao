using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Pagamentos.Api.Data;

namespace PlataformaEducacional.Pagamentos.Api.Configuration.Seed
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
                    var context = services.GetRequiredService<PagamentosContext>();

                    await context.Database.MigrateAsync();

                    await EnsureSeedPagamentos(context);
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating the database.");
            }
        }

        private static async Task EnsureSeedPagamentos(PagamentosContext context)
        {
            if (await context.Pagamentos.AnyAsync())
                return;
            
            //TODO: add seed de pagamentos
            //var pagamento = new Pagamento(Guid.Parse("65EFB6D9-2374-4E87-8D83-C8E76C2B9765"), "Aluno Teste", "aluno.teste@educa.com", "32009883985");
            //context.Pagamentos.Add(pagamento);
            //await context.SaveChangesAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Pedidos.Data;

namespace PlataformaEducacional.Pedidos.API.Configuration
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
                    var context = services.GetRequiredService<PedidosContext>();

                    await context.Database.MigrateAsync();

                    await EnsureSeedPedidos(context);
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating the database.");
            }
        }

        private static async Task EnsureSeedPedidos(PedidosContext context)
        {
            //if (await context.Alunos.AnyAsync())
            //    return;

            //var aluno = new Aluno(Guid.Parse("65EFB6D9-2374-4E87-8D83-C8E76C2B9765"), "Aluno Teste", "aluno.teste@educa.com", "32009883985");

            //var enderecoCommand = new AdicionarEnderecoCommand(
            //    "Rua Teste", "123", "Apto 101", "Bairro Teste", "12345678", "Cidade Teste", "SP")
            //{
            //    AlunoId = aluno.Id
            //};

            //var endereco = new Endereco(enderecoCommand);
            //aluno.AtribuirEndereco(endereco);

            //context.Alunos.Add(aluno);

            await context.SaveChangesAsync();
        }
    }

}

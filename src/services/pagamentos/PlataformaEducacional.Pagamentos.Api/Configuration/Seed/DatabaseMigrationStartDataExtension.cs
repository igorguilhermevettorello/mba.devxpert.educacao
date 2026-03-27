using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Pagamentos.Api.Data;
using PlataformaEducacional.Pagamentos.Api.Models;
using PlataformaEducacional.Pagamentos.Api.Models.Enums;
using PlataformaEducacional.Pagamentos.Api.Models.ValueObjects;

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

            var pagamento = GerarPagamento();
            context.Pagamentos.Add(pagamento);
            await context.SaveChangesAsync();
        }

        private static Pagamento GerarPagamento()
        {
            var cartaoCredito = CartaoCredito.Criar("Joaozinho da Silva", "5571 9114 0412 6886", "02/27", "690");
            var pagamento = new Pagamento(Guid.NewGuid(), TipoPagamento.CartaoCredito, 1500M, cartaoCredito);
            pagamento.AdicionarTransacao(GerarTransacao(pagamento.Id));
            return pagamento;
        }

        private static Transacao GerarTransacao(Guid pagamentoId)
        {
            var transacao = new Transacao(
                "JDPL2DUAPM",
                "MasterCard",
                DateTime.Now,
                1500M,
                2.4M,
                StatusTransacao.Autorizado,
                "B3DO4XP9XW",
                "4W6FYCOWMQ",
                pagamentoId);

            return transacao;
        }
    }
}

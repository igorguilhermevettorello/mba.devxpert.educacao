using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Pagamentos.Api.Data;
using PlataformaEducacional.Pagamentos.Api.Models;
using PlataformaEducacional.Pagamentos.Api.Models.Enums;
using PlataformaEducacional.Pagamentos.Api.Models.ValueObjects;
using PlataformaEducacional.SeedDados;

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

            var pagamento = GerarPagamento(
                SeedMatriculas.MATRICULA_ALUNO1_ID,
                299.90M,
                "João Estudioso",
                "4539595764370442",
                "09/26",
                "982");

            context.Pagamentos.Add(pagamento);
            await context.SaveChangesAsync();
        }

        private static Pagamento GerarPagamento(
            Guid matriculaId,
            decimal valor,
            string titular,
            string numeroCartao,
            string validade,
            string cvv)
        {
            var cartaoCredito = CartaoCredito.TryCreate(titular, numeroCartao, validade, cvv);
            var pagamento = new Pagamento(matriculaId, TipoPagamento.CartaoCredito, valor, cartaoCredito.Card!);

            // Adiciona transação ao pagamento
            pagamento.AdicionarTransacao(GerarTransacao(pagamento.Id));

            return pagamento;
        }

        private static Transacao GerarTransacao(Guid pagamentoId)
        {
            // Gera IDs aleatórios para TID e NSU (como gerados pelo gateway)
            var tid = GerarCaminhoAlfanumerico(12);
            var nsu = GerarCaminhoAlfanumerico(12);
            var codigoAutorizacao = GerarCaminhoAlfanumerico(10);

            var transacao = new Transacao(
                codigoAutorizacao,
                "Visa", // Bandeira do cartão
                DateTime.UtcNow,
                299.90M,
                2.4M, // Taxa de transação em %
                StatusTransacao.Autorizado,
                tid,
                nsu,
                pagamentoId);

            return transacao;
        }

        private static string GerarCaminhoAlfanumerico(int tamanho)
        {
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Range(0, tamanho)
                .Select(_ => caracteres[random.Next(caracteres.Length)])
                .ToArray());
        }
    }
}

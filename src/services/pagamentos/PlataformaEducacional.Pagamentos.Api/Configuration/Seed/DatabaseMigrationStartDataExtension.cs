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

            var pagamentos = new List<Pagamento>
            {
                // Pagamento 1: Aluno Teste - C# Fundamentos (R$ 299,90)
                GerarPagamento(
                    Guid.Parse("75EFB6D9-2374-4E87-8D83-C8E76C2B9766"), 
                    299.90M, 
                    "João da Silva",
                    "4539595764370442",
                    "09/26",
                    "982")
                // Pagamento 2: Aluno Teste - ASP.NET Core Web API (R$ 399,90)
                // GerarPagamento(
                //     Guid.Parse("65EFB6D9-2374-4E87-8D83-C8E76C2B9765"), 
                //     399.90M, 
                //     "João da Silva",
                //     "4539003370725167",
                //     "05/28",
                //     "234"),

                // Pagamento 3: João Silva - C# Fundamentos (R$ 299,90)
                //GerarPagamento(
                //     Guid.Parse("75EFB6D9-2374-4E87-8D83-C8E76C2B9766"), 
                //     299.90M, 
                //     "João Silva",
                //     "5573100000000019",
                //     "12/26",
                //     "123"),
                //
                // // Pagamento 4: João Silva - Clean Architecture (R$ 599,90)
                // GerarPagamento(
                //     Guid.Parse("75EFB6D9-2374-4E87-8D83-C8E76C2B9766"), 
                //     599.90M, 
                //     "João Silva",
                //     "6011111111111117",
                //     "08/27",
                //     "456"),
                //
                // // Pagamento 5: Maria Santos - ASP.NET Core Web API (R$ 399,90)
                // GerarPagamento(
                //     Guid.Parse("85EFB6D9-2374-4E87-8D83-C8E76C2B9767"), 
                //     399.90M, 
                //     "Maria Santos",
                //     "3714890050000000",
                //     "03/28",
                //     "789"),
                //
                // // Pagamento 6: Maria Santos - SQL Server e EF Core (R$ 349,90)
                // GerarPagamento(
                //     Guid.Parse("85EFB6D9-2374-4E87-8D83-C8E76C2B9767"), 
                //     349.90M, 
                //     "Maria Santos",
                //     "5400111111111115",
                //     "06/27",
                //     "321"),
                //
                // // Pagamento 7: Pedro Oliveira - SQL Server e EF Core (R$ 349,90)
                // GerarPagamento(
                //     Guid.Parse("95EFB6D9-2374-4E87-8D83-C8E76C2B9768"), 
                //     349.90M, 
                //     "Pedro Oliveira",
                //     "6011000990139424",
                //     "09/26",
                //     "654"),
                //
                // // Pagamento 8: Pedro Oliveira - Testes Automatizados (R$ 279,90)
                // GerarPagamento(
                //     Guid.Parse("95EFB6D9-2374-4E87-8D83-C8E76C2B9768"), 
                //     279.90M, 
                //     "Pedro Oliveira",
                //     "4024007134432509",
                //     "11/27",
                //     "987"),
            };

            context.Pagamentos.AddRange(pagamentos);
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
            var cartaoCredito = CartaoCredito.Criar(titular, numeroCartao, validade, cvv);
            var pagamento = new Pagamento(matriculaId, TipoPagamento.CartaoCredito, valor, cartaoCredito);
            
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
                0M, // Valor será preenchido pelo gateway
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

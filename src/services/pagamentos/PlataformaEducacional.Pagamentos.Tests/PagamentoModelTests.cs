using System;
using PlataformaEducacional.Pagamentos.Api.Models;
using PlataformaEducacional.Pagamentos.Api.Models.ValueObjects;
using PlataformaEducacional.Pagamentos.Api.Models.Enums;
using Xunit;

namespace PlataformaEducacional.Pagamentos.Api.Tests
{
    public class PagamentoModelTests
    {
        [Fact]
        public void Ctor_SetsProperties_And_AddTransacao()
        {
            var matriculaId = Guid.NewGuid();
            var (isValid, card, _) = CartaoCredito.TryCreate("Teste", "4111111111111111", "12/30", "123");
            Assert.True(isValid);
            var pagamento = new Pagamento(matriculaId, TipoPagamento.CartaoCredito, 100m, card!);

            Assert.Equal(matriculaId, pagamento.MatriculaId);
            Assert.Equal(100m, pagamento.Valor);
            Assert.NotNull(pagamento.CartaoCredito);
            Assert.Empty(pagamento.Transacoes);

            var trans = new Transacao("auth", "Visa", DateTime.UtcNow, 100m, 1.2m, StatusTransacao.Autorizado, "TID1", "NSU1");
            pagamento.AdicionarTransacao(trans);

            Assert.Single(pagamento.Transacoes);
            Assert.Equal(trans, System.Linq.Enumerable.First(pagamento.Transacoes));
        }
    }
}
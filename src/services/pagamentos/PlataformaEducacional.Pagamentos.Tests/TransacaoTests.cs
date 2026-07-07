using System;
using PlataformaEducacional.Pagamentos.Api.Models;
using PlataformaEducacional.Pagamentos.Api.Models.Enums;
using Xunit;

namespace PlataformaEducacional.Pagamentos.Api.Tests
{
    public class TransacaoTests
    {
        [Fact]
        public void Ctor_SetsProperties_And_AtualizarPagamentoId()
        {
            var pagamentoId = Guid.NewGuid();
            var trans = new Transacao("auth", "Visa", DateTime.UtcNow, 200m, 2.4m, StatusTransacao.Autorizado, "TIDX", "NSUX", pagamentoId);

            Assert.Equal("auth", trans.CodigoAutorizacao);
            Assert.Equal("Visa", trans.BandeiraCartao);
            Assert.Equal(200m, trans.ValorTotal);
            Assert.Equal(StatusTransacao.Autorizado, trans.Status);
            Assert.Equal(pagamentoId, trans.PagamentoId);

            var newPagamentoId = Guid.NewGuid();
            trans.AtualizarPagamentoId(newPagamentoId);

            Assert.Equal(newPagamentoId, trans.PagamentoId);
        }
    }
}
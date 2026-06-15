using System;
using FluentValidation.Results;
using PlataformaEducacional.Pagamentos.Api.Models.ValueObjects;
using Xunit;

namespace PlataformaEducacional.Pagamentos.Api.Tests
{
    public class CartaoCreditoTests
    {
        [Fact]
        public void TryCreate_ReturnsValid_ForKnownGoodCard()
        {
            // Use widely-known test Visa number
            var (isValid, card, validation) = CartaoCredito.TryCreate("João Silva", "4111111111111111", "12/30", "123");

            Assert.True(isValid);
            Assert.NotNull(card);
            Assert.IsType<ValidationResult>(validation);
        }

        [Fact]
        public void TryCreate_ReturnsInvalid_ForExpiredCard()
        {
            var (isValid, card, validation) = CartaoCredito.TryCreate("João Silva", "4111111111111111", "01/20", "123");

            Assert.False(isValid);
            Assert.Null(card);
            Assert.False(validation.IsValid);
        }

        [Fact]
        public void TryCreate_ReturnsInvalid_ForInvalidNumber()
        {
            var (isValid, card, validation) = CartaoCredito.TryCreate("João Silva", "1234567890123456", "12/30", "123");

            Assert.False(isValid);
            Assert.Null(card);
            Assert.False(validation.IsValid);
        }
    }
}
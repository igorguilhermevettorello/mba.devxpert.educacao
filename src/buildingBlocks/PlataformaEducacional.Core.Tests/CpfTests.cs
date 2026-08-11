using Xunit;
using PlataformaEducacional.Core.DomainObjects;

namespace PlataformaEducacional.Core.Tests
{
    public class CpfTests
    {
        [Fact]
        public void Validar_ReturnsTrue_ForValidCpf_Unformatted()
        {
            var valid = "52998224725"; // known valid CPF
            Assert.True(Cpf.Validar(valid));
        }

        [Fact]
        public void Validar_ReturnsTrue_ForValidCpf_Formatted()
        {
            var formatted = "529.982.247-25"; // same valid CPF with punctuation
            Assert.True(Cpf.Validar(formatted));
        }

        [Fact]
        public void Validar_ReturnsFalse_ForAllDigitsEqual()
        {
            var allEqual = "111.111.111-11";
            Assert.False(Cpf.Validar(allEqual));
        }

        [Fact]
        public void Validar_ReturnsFalse_ForKnownInvalidSequence()
        {
            var invalid = "12345678909"; // explicitly rejected by implementation
            Assert.False(Cpf.Validar(invalid));
        }

        [Fact]
        public void Validar_ReturnsFalse_When_TooLong()
        {
            var tooLong = "529982247259"; // 12 digits
            Assert.False(Cpf.Validar(tooLong));
        }

        [Fact]
        public void Ctor_SetsNumero_ForValidCpf()
        {
            var raw = "52998224725";
            var cpf = new Cpf(raw);
            Assert.Equal(raw, cpf.Numero);
        }

        [Fact]
        public void Ctor_ThrowsDomainException_ForInvalidCpf()
        {
            var invalid = "11111111111";
            Assert.Throws<PlataformaEducacional.Core.DomainObjects.DomainException>(() => new Cpf(invalid));
        }
    }
}
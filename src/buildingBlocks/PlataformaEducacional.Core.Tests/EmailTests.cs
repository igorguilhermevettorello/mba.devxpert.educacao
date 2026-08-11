using System;
using Xunit;
using PlataformaEducacional.Core.DomainObjects;

namespace PlataformaEducacional.Core.Tests
{
    public class EmailTests
    {
        [Fact]
        public void Validar_ReturnsTrue_ForValidEmail_Simple()
        {
            Assert.True(Email.Validar("user@example.com"));
        }

        [Fact]
        public void Validar_ReturnsTrue_ForValidEmail_WithPlusAndDots()
        {
            Assert.True(Email.Validar("first.last+tag@example.co.uk"));
        }

        [Fact]
        public void Validar_ReturnsFalse_ForInvalidEmail_NoAt()
        {
            Assert.False(Email.Validar("invalid-email"));
        }

        [Fact]
        public void Validar_ReturnsFalse_ForEmptyString()
        {
            Assert.False(Email.Validar(string.Empty));
        }

        [Fact]
        public void Validar_ThrowsArgumentNullException_ForNullInput()
        {
            Assert.Throws<ArgumentNullException>(() => Email.Validar(null!));
        }

        [Fact]
        public void Ctor_SetsEndereco_ForValidEmail()
        {
            var endereco = "user@example.com";
            var email = new Email(endereco);
            Assert.Equal(endereco, email.Endereco);
        }

        [Fact]
        public void Ctor_ThrowsDomainException_ForInvalidEmail()
        {
            Assert.Throws<PlataformaEducacional.Core.DomainObjects.DomainException>(() => new Email("not-an-email"));
        }
    }
}
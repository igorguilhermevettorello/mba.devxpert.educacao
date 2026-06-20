using System;
using PlataformaEducacional.Alunos.Domain.Models;
using Xunit;

namespace PlataformaEducacional.Alunos.Tests
{
    public class CertificadoTests
    {
        [Fact]
        public void Ctor_SetsFields()
        {
            var matriculaId = Guid.NewGuid();
            var cert = new Certificado(matriculaId);

            Assert.Equal(matriculaId, cert.MatriculaId);
            Assert.NotEqual(Guid.Empty, cert.CodigoValidacao);
            Assert.True((DateTime.UtcNow - cert.DataEmissao).TotalSeconds < 10);
        }
    }
}
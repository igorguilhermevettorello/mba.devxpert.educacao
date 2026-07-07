using System;
using PlataformaEducacional.Alunos.Domain.Models;
using Xunit;

namespace PlataformaEducacional.Alunos.Tests
{
    public class ProgressoAulaTests
    {
        [Fact]
        public void Ctor_SetsProperties()
        {
            var matriculaId = Guid.NewGuid();
            var aulaId = Guid.NewGuid();

            var p = new ProgressoAula(matriculaId, aulaId);

            Assert.Equal(matriculaId, p.MatriculaId);
            Assert.Equal(aulaId, p.AulaId);
            Assert.True((DateTime.UtcNow - p.DataConclusao).TotalSeconds < 10);
        }
    }
}
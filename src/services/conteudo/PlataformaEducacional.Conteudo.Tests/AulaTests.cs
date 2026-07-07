using System;
using PlataformaEducacional.Conteudo.Domain.Entities;
using Xunit;

namespace PlataformaEducacional.Conteudo.Api.Tests
{
    public class AulaTests
    {
        [Fact]
        public void Ctors_SetProperties()
        {
            var a = new Aula("T", "D", 40, 1);
            Assert.Equal("T", a.Titulo);
            Assert.Equal("D", a.Descricao);
            Assert.Equal(40, a.DuracaoMinutos);
            Assert.Equal(1, a.Ordem);
            Assert.True(a.Ativa);
        }

        [Fact]
        public void AtualizarCampos_Works()
        {
            var a = new Aula("T", "D", 40, 1);
            a.AtualizarTitulo("T2");
            a.AtualizarDescricao("D2");
            a.AtualizarDuracao(55);
            a.AtualizarOrdem(2);

            Assert.Equal("T2", a.Titulo);
            Assert.Equal("D2", a.Descricao);
            Assert.Equal(55, a.DuracaoMinutos);
            Assert.Equal(2, a.Ordem);
        }

        [Fact]
        public void Inativar_Ativar()
        {
            var a = new Aula("T", "D", 40, 1);
            a.Inativar();
            Assert.False(a.Ativa);
            a.Ativar();
            Assert.True(a.Ativa);
        }

        [Fact]
        public void AssociarCurso_Validations()
        {
            var a = new Aula("T", "D", 40, 1);

            Assert.Throws<ArgumentException>(() => a.AssociarCurso(Guid.Empty));

            var cursoId = Guid.NewGuid();
            a.AssociarCurso(cursoId);
            Assert.Equal(cursoId, a.CursoId);

            // associar a outro curso deve lançar
            var outro = Guid.NewGuid();
            Assert.Throws<InvalidOperationException>(() => a.AssociarCurso(outro));
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(50, false)]
        [InlineData(100, true)]
        [InlineData(150, true)]
        public void EstaConcluida_ReturnsExpected(int progresso, bool esperado)
        {
            var a = new Aula("T", "D", 10, 1);
            Assert.Equal(esperado, a.EstaConcluida(progresso));
        }
    }
}
using System;
using System.Linq;
using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Conteudo.Domain.ValueObjects;
using PlataformaEducacional.WebApi.Core.Enumerators;
using Xunit;

namespace PlataformaEducacional.Conteudo.Api.Tests
{
    public class CursoTests
    {
        [Fact]
        public void Ctor_InitializesProperties()
        {
            var curso = new Curso("T", "D", "I", NivelCurso.Basico, 10m);

            Assert.Equal("T", curso.Titulo);
            Assert.Equal("D", curso.Descricao);
            Assert.Equal("I", curso.Instrutor);
            Assert.Equal(NivelCurso.Basico, curso.Nivel);
            Assert.True(curso.Ativo);
            Assert.NotNull(curso.Aulas);
            Assert.Equal(0, curso.ObterTotalAulas());
        }

        [Fact]
        public void AdicionarAula_AddsAula_WhenCursoAtivo()
        {
            var curso = new Curso("T", "D", "I", NivelCurso.Basico, 10m);
            var aula = new Aula("A1", "desc", 30, 1);

            curso.AdicionarAula(aula);

            Assert.True(curso.VerificarSeAulaEstaCadastrada(aula.Id));
            Assert.Equal(1, curso.ObterTotalAulas());
            Assert.Equal(30, curso.ObterDuracaoTotalMinutos());
        }

        [Fact]
        public void AdicionarAula_Throws_WhenCursoInativo()
        {
            var curso = new Curso("T", "D", "I", NivelCurso.Basico, 10m);
            curso.Inativar();
            var aula = new Aula("A1", "desc", 30, 1);

            Assert.Throws<InvalidOperationException>(() => curso.AdicionarAula(aula));
        }

        [Fact]
        public void RemoverAula_And_ObterAulaPorId_Behavior()
        {
            var curso = new Curso("T", "D", "I", NivelCurso.Basico, 10m);
            var aula = new Aula(Guid.NewGuid(), "A1", "desc", 20, 1);
            curso.AdicionarAula(aula);

            var fetched = curso.ObterAulaPorId(aula.Id);
            Assert.Equal(aula.Id, fetched.Id);

            curso.RemoverAula(aula.Id);
            Assert.Equal(0, curso.ObterTotalAulas());

            Assert.Throws<InvalidOperationException>(() => curso.ObterAulaPorId(aula.Id));
            Assert.Throws<InvalidOperationException>(() => curso.RemoverAula(aula.Id));
        }

        [Fact]
        public void AdicionarConteudoProgramatico_And_Atualizar()
        {
            var curso = new Curso("T", "D", "I", NivelCurso.Basico, 10m);
            var conteudo = new ConteudoProgramatico("ementa suficiente", "objetivo suficiente", "bibliografia suficiente", "https://example.com");

            curso.AdicionarConteudoProgramatico(conteudo);
            Assert.NotNull(curso.ConteudoProgramatico);

            var novo = new ConteudoProgramatico("ementa nova suficiente", "objetivo novo", "biblio nova", "https://example.com/new");
            curso.AtualizarConteudoProgramatico(novo);
            Assert.Equal("ementa nova suficiente", curso.ConteudoProgramatico.Ementa);
        }

        [Fact]
        public void AtualizarInformacoes_E_AtualizarCampos()
        {
            var curso = new Curso("T", "D", "I", NivelCurso.Basico, 10m);
            curso.AtualizarInformacoes("T2", "D2", "I2", NivelCurso.Avancado, 99m);

            Assert.Equal("T2", curso.Titulo);
            Assert.Equal("D2", curso.Descricao);
            Assert.Equal("I2", curso.Instrutor);
            Assert.Equal(NivelCurso.Avancado, curso.Nivel);
            Assert.Equal(99m, curso.Valor);
        }

        [Fact]
        public void Inativar_And_Ativar_And_AtualizarNivel()
        {
            var curso = new Curso("T", "D", "I", NivelCurso.Basico, 10m);

            curso.Inativar();
            Assert.False(curso.Ativo);

            curso.Ativar();
            Assert.True(curso.Ativo);

            curso.AtualizarNivel(NivelCurso.Intermediario);
            Assert.Equal(NivelCurso.Intermediario, curso.Nivel);
        }
    }
}
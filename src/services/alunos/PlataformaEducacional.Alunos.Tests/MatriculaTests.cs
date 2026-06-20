using System;
using PlataformaEducacional.Alunos.Domain.Models;
using PlataformaEducacional.WebApi.Core.Enumerators;
using Xunit;

namespace PlataformaEducacional.Alunos.Tests
{
    public class MatriculaTests
    {
        [Fact]
        public void Ctor_SetsProperties()
        {
            var alunoId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();

            var m = new Matricula(alunoId, cursoId);

            Assert.NotEqual(Guid.Empty, m.Id);
            Assert.Equal(alunoId, m.AlunoId);
            Assert.Equal(cursoId, m.CursoId);
            Assert.Equal(StatusMatricula.Pendente, m.Status);
            Assert.NotNull(m.ProgressoAulas);
        }

        [Fact]
        public void Ativar_Cancelar_Concluir_ChangeStatus()
        {
            var m = new Matricula(Guid.NewGuid(), Guid.NewGuid());

            m.Ativar();
            Assert.Equal(StatusMatricula.Ativa, m.Status);

            m.Cancelar();
            Assert.Equal(StatusMatricula.Cancelada, m.Status);

            m.Concluir();
            Assert.Equal(StatusMatricula.Concluida, m.Status);
        }

        [Fact]
        public void AdicionarProgresso_AddsItem()
        {
            var m = new Matricula(Guid.NewGuid(), Guid.NewGuid());
            var progresso = new ProgressoAula(m.Id, Guid.NewGuid());

            m.AdicionarProgresso(progresso);

            Assert.Contains(progresso, m.ProgressoAulas);
        }

        [Fact]
        public void AdicionarCertificado_AssignsCertificado()
        {
            var m = new Matricula(Guid.NewGuid(), Guid.NewGuid());
            var cert = new Certificado(m.Id);

            m.AdicionarCertificado(cert);

            Assert.Equal(cert, m.Certificado);
        }
    }
}
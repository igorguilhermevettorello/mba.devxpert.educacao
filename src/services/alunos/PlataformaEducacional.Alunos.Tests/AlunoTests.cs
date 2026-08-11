using System;
using System.Linq;
using PlataformaEducacional.Alunos.Domain.Models;
using PlataformaEducacional.Core.DomainObjects;
using Xunit;

namespace PlataformaEducacional.Alunos.Tests
{
    public class AlunoTests
    {
        [Fact]
        public void Ctor_InitializesProperties()
        {
            var id = Guid.NewGuid();
            var aluno = new Aluno(id, "Nome", "email@example.com", "09044871056");

            Assert.Equal(id, aluno.Id);
            Assert.Equal("Nome", aluno.Nome);
            Assert.Equal("email@example.com", aluno.Email.Endereco);
            Assert.Equal("09044871056", aluno.Cpf.Numero);
            Assert.False(aluno.Excluido);
            Assert.NotNull(aluno.Matriculas);
        }

        [Fact]
        public void TrocarEmail_UpdatesEmail()
        {
            var aluno = new Aluno(Guid.NewGuid(), "Nome", "old@example.com", "09044871056");
            aluno.TrocarEmail("new@example.com");
            Assert.Equal("new@example.com", aluno.Email.Endereco);
        }

        [Fact]
        public void AtribuirEndereco_AssignsEndereco()
        {
            var aluno = new Aluno(Guid.NewGuid(), "Nome", "email@example.com", "09044871056");
            var endereco = new Endereco("R", "1", "", "B", "00000-000", "C", "SP", aluno.Id);

            aluno.AtribuirEndereco(endereco);

            Assert.Equal(endereco, aluno.Endereco);
        }

        [Fact]
        public void Certificados_ReflectsMatriculas()
        {
            var alunoId = Guid.NewGuid();
            var aluno = new Aluno(alunoId, "Nome", "email@example.com", "09044871056");

            var m1 = new Matricula(Guid.NewGuid(), alunoId, Guid.NewGuid());
            var m2 = new Matricula(Guid.NewGuid(), alunoId, Guid.NewGuid());
            var cert = new Certificado(m1.Id);
            m1.AdicionarCertificado(cert);

            // attach matriculas via reflection because collection is readonly and internal to Aggregate
            var field = typeof(Aluno).GetField("_matriculas", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var list = (System.Collections.IList?)field!.GetValue(aluno) ?? throw new InvalidOperationException("field not found");
            list.Add(m1);
            list.Add(m2);

            var certificados = aluno.Certificados.ToList();
            Assert.Single(certificados);
            Assert.Equal(cert, certificados[0]);
        }
    }
}
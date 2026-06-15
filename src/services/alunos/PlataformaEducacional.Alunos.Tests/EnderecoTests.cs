using System;
using PlataformaEducacional.Alunos.Domain.Models;
using Xunit;

namespace PlataformaEducacional.Alunos.Tests
{
    public class EnderecoTests
    {
        [Fact]
        public void Ctor_SetsAllProperties()
        {
            var alunoId = Guid.NewGuid();
            var endereco = new Endereco("Rua A", "123", "Apto 1", "Bairro", "00000000", "Cidade", "SP", alunoId);

            Assert.Equal("Rua A", endereco.Logradouro);
            Assert.Equal("123", endereco.Numero);
            Assert.Equal("Apto 1", endereco.Complemento);
            Assert.Equal("Bairro", endereco.Bairro);
            Assert.Equal("00000000", endereco.Cep);
            Assert.Equal("Cidade", endereco.Cidade);
            Assert.Equal("SP", endereco.Estado);
            Assert.Equal(alunoId, endereco.AlunoId);
        }
    }
}
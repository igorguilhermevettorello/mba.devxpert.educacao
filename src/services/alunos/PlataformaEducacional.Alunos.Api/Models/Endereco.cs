using PlataformaEducacional.Alunos.Api.Application.Commands;
using PlataformaEducacional.Core.DomainObjects;

namespace PlataformaEducacional.Alunos.Api.Models;

public class Endereco : Entity
{
    protected Endereco() { }

    public Endereco(AdicionarEnderecoCommand command)
    {
        Logradouro = command.Logradouro;
        Numero = command.Numero;
        Complemento = command.Complemento;
        Bairro = command.Bairro;
        Cep = command.Cep;
        Cidade = command.Cidade;
        Estado = command.Estado;
        AlunoId = command.AlunoId;
    }

    public string Logradouro { get; private set; }
    public string Numero { get; private set; }
    public string Complemento { get; private set; }
    public string Bairro { get; private set; }
    public string Cep { get; private set; }
    public string Cidade { get; private set; }
    public string Estado { get; private set; }
    public Guid AlunoId { get; private set; }

    public Aluno Aluno { get; protected set; }
    
}

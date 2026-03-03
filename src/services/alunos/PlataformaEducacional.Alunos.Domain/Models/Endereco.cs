using PlataformaEducacional.Core.DomainObjects;


namespace PlataformaEducacional.Alunos.Domain.Models;

public class Endereco : Entity
{
    protected Endereco() { }

    public Endereco(string logradouro, string numero, string complemento, string bairro, string cep, string cidade, string estado, Guid alunoId)
    {
        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;
        Bairro = bairro;
        Cep = cep;
        Cidade = cidade;
        Estado = estado;
        AlunoId = alunoId;
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

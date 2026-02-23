using PlataformaEducacional.Core.DomainObjects;

namespace PlataformaEducacional.Alunos.Api.Models;

public class Aluno : Entity, IAggregateRoot
{
    private readonly List<Matricula> _matriculas;

    protected Aluno() 
    { 
        _matriculas = new List<Matricula>();
    }

    public Aluno(Guid id, string nome, string email, string cpf)
    {
        Id = id;
        Nome = nome;
        Email = new Email(email);
        Cpf = new Cpf(cpf);
        Excluido = false;
        _matriculas = new List<Matricula>();
    }

    public string Nome { get; private set; }
    public Email Email { get; private set; }
    public Cpf Cpf { get; private set; }
    public bool Excluido { get; private set; }

    public Endereco Endereco { get; private set; }
    public IReadOnlyCollection<Matricula> Matriculas => _matriculas;

    public void TrocarEmail(string email)
    {
        Email = new Email(email);
    }

    public void AtribuirEndereco(Endereco endereco)
    {
        Endereco = endereco;
    }
}

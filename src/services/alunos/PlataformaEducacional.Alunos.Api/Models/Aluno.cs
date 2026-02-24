using PlataformaEducacional.Core.DomainObjects;

namespace PlataformaEducacional.Alunos.Api.Models;

public class Aluno : Entity, IAggregateRoot
{

    protected Aluno() 
    { 
        _matriculas = new List<Matricula>();
        _certificados = new List<Certificado>();
    }

    public Aluno(Guid id, string nome, string email, string cpf)
    {
        Id = id;
        Nome = nome;
        Email = new Email(email);
        Cpf = new Cpf(cpf);
        Excluido = false;
        _matriculas = new List<Matricula>();
        _certificados = new List<Certificado>();
    }

    public string Nome { get; private set; }
    public Email Email { get; private set; }
    public Cpf Cpf { get; private set; }
    public bool Excluido { get; private set; }

    public Endereco Endereco { get; private set; }

    private readonly List<Matricula> _matriculas;
    public IReadOnlyCollection<Matricula> Matriculas => _matriculas;

    private readonly IList<Certificado> _certificados;
    public IReadOnlyCollection<Certificado> Certificados => _certificados.ToList();

    public void TrocarEmail(string email)
    {
        Email = new Email(email);
    }

    public void AtribuirEndereco(Endereco endereco)
    {
        Endereco = endereco;
    }
}

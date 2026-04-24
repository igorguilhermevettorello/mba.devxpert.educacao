using PlataformaEducacional.Core.DomainObjects;
using PlataformaEducacional.WebApi.Core.Enumerators;

namespace PlataformaEducacional.Alunos.Domain.Models;

public class Matricula : Entity
{
    protected Matricula()
    {
        _progressoAulas = new List<ProgressoAula>();
    }

    public Matricula(Guid alunoId, Guid cursoId) : this()
    {
        Id = Guid.NewGuid();
        AlunoId = alunoId;
        CursoId = cursoId;
        DataMatricula = DateTime.UtcNow;
        Status = StatusMatricula.Pendente;
        _progressoAulas = new List<ProgressoAula>();
    }

    //construtor usado no seed
    public Matricula(Guid id, Guid alunoId, Guid cursoId) : this()
    {
        Id = id;
        AlunoId = alunoId;
        CursoId = cursoId;
        DataMatricula = DateTime.UtcNow;
        Status = StatusMatricula.Pendente;
        _progressoAulas = new List<ProgressoAula>();
    }

    public Guid AlunoId { get; private set; }
    public Guid CursoId { get; private set; }
    public DateTime DataMatricula { get; private set; }
    public StatusMatricula Status { get; private set; }

    public Aluno Aluno { get; protected set; }

    public Certificado Certificado { get; protected set; }

    private readonly List<ProgressoAula> _progressoAulas;
    public IReadOnlyCollection<ProgressoAula> ProgressoAulas => _progressoAulas;


    public void Ativar()
    {
        Status = StatusMatricula.Ativa;
    }
    public void Cancelar()
    {
        Status = StatusMatricula.Cancelada;
    }
    public void Concluir()
    {
        Status = StatusMatricula.Concluida;
    }
    public void AdicionarProgresso(ProgressoAula progresso)
    {
        _progressoAulas.Add(progresso);
    }

    public void AdicionarCertificado(Certificado certificado)
    {
        Certificado = certificado;
    }
}

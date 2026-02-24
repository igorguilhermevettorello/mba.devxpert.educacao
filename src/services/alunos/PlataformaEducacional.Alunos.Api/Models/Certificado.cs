using PlataformaEducacional.Core.DomainObjects;

namespace PlataformaEducacional.Alunos.Api.Models;

public class Certificado : Entity
{
    protected Certificado() { }
    
    public Certificado(Guid matriculaId)
    {
        MatriculaId = matriculaId;
        CodigoValidacao = Guid.NewGuid();
        DataEmissao = DateTime.UtcNow;
    }

    public Guid MatriculaId { get; private set; }
    public Guid CodigoValidacao { get; private set; }
    public DateTime DataEmissao { get; private set; }
    public Matricula Matricula { get; protected set; }
    public Aluno Aluno { get; protected set; }
}

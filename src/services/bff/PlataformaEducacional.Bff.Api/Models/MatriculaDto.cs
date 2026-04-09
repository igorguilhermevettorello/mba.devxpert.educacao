using PlataformaEducacional.WebApi.Core.Enumerators;

namespace PlataformaEducacional.Bff.Api.Models;

public class MatriculaDto
{
    public Guid Id { get; set; }
    public Guid AlunoId { get; private set; }
    public Guid CursoId { get; private set; }
    public DateTime DataMatricula { get; private set; }
    public StatusMatricula Status { get; private set; }

    public AlunoDto Aluno { get; protected set; } = null!;
}

using PlataformaEducacional.WebApi.Core.Enumerators;

namespace PlataformaEducacional.Bff.Api.Models;

public class MatriculaDto
{
    public Guid Id { get; set; }
    public Guid AlunoId { get; set; }
    public Guid CursoId { get; set; }
    public DateTime DataMatricula { get; set; }
    public StatusMatricula Status { get; set; }

    public AlunoDto Aluno { get; protected set; } = null!;
}

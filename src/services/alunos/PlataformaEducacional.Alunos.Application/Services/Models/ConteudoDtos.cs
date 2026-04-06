namespace PlataformaEducacional.Alunos.Application.Services.Models;

public class CursoDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }
}

public class AulaDto
{
    public Guid Id { get; set; }
    public Guid CursoId { get; set; }
    public string Titulo { get; set; }
}

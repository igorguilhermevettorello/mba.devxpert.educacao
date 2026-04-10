using PlataformaEducacional.WebApi.Core.Enumerators;

namespace PlataformaEducacional.Bff.Api.Models;

public class CursoDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Instrutor { get; set; } = string.Empty;
    public NivelCurso Nivel { get; set; }
    public decimal Valor { get; set; }
    public bool Ativo { get; set; }
    public ConteudoProgramaticoDto? ConteudoProgramatico { get; set; }
}

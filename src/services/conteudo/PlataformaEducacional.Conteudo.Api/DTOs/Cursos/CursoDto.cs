using PlataformaEducacional.Conteudo.Api.DTOs.Aulas;
using PlataformaEducacional.Conteudo.Api.DTOs.ConteudoProgramatico;
using PlataformaEducacional.WebApi.Core.Enumerators;

namespace PlataformaEducacional.Conteudo.Api.DTOs.Cursos;

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
    public IEnumerable<AulaDto> Aulas { get; set; } = new List<AulaDto>();
}

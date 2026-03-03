using PlataformaEducacional.Conteudo.Api.DTOs.Aulas;
using PlataformaEducacional.Conteudo.Domain.Enums;

namespace PlataformaEducacional.Conteudo.Api.DTOs.Cursos
{
    public class CursoDto
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Instrutor { get; set; }
        public NivelCurso Nivel { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool Ativo { get; set; }
        public ConteudoProgramaticoDto? ConteudoProgramatico { get; set; }
        public IEnumerable<AulaDto> Aulas { get; set; } = new List<AulaDto>();
    }
}

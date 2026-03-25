namespace PlataformaEducacional.Conteudo.Api.DTOs.Aulas
{
    public class AulaDto
    {
        public Guid Id { get; set; }
        public Guid CursoId { get; set; }
        public string Titulo { get; set; } = default!;
        public string Descricao { get; set; } = default!;
        public int DuracaoMinutos { get; set; }
        public int Ordem { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}

using PlataformaEducacional.Pagamentos.Api.Models.Enums;

namespace PlataformaEducacional.Pagamentos.Api.Models.DTOs
{
    public class PagamentoDto
    {
        public Guid Id { get; set; }
        public Guid MatriculaId { get; set; }
        public TipoPagamento TipoPagamento { get; set; }
        public decimal Valor { get; set; }


        public ICollection<TransacaoDto>? Transacoes { get; set; }
    }
}

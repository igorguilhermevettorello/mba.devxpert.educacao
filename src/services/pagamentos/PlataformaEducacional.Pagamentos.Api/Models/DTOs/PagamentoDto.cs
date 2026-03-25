using PlataformaEducacional.Core.DomainObjects;

namespace PlataformaEducacional.Pagamentos.Api.Models.DTOs
{
    public class PagamentoDto
    {
        public Guid Id { get; set; }
        public Guid MatriculaId { get; set; }
        public TipoPagamento TipoPagamento { get; set; }
        public decimal Valor { get; set; }

        //public CartaoCredito CartaoCredito { get; set; }  //TODO: avaliar se é conveniente exibir dados do cartão

        public ICollection<TransacaoDto>? Transacoes { get; set; }
    }
}

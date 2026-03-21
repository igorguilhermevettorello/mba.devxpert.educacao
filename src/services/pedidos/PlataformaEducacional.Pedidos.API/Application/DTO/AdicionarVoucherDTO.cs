using PlataformaEducacional.Pedidos.Domain.Vouchers;

namespace PlataformaEducacional.Pedidos.API.Application.DTO
{
    public class AdicionarVoucherDTO
    {
        public string Codigo { get; set; }
        public decimal? Percentual { get; set; }
        public decimal? ValorDesconto { get; set; }
        public int Quantidade { get; set; }
        public TipoDescontoVoucher TipoDesconto { get; set; }
        public DateTime DataValidade { get; set; }
    }
}
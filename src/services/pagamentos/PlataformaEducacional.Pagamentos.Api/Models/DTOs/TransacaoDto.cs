using PlataformaEducacional.Pagamentos.Api.Models.Enums;

namespace PlataformaEducacional.Pagamentos.Api.Models.DTOs
{
    public class TransacaoDto
    {
        public string CodigoAutorizacao { get; set; } = string.Empty;
        public string BandeiraCartao { get; set; } = string.Empty;
        public DateTime? DataTransacao { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal CustoTransacao { get; set; }
        public StatusTransacao Status { get; set; }
        public string TID { get; set; }
        public string NSU { get; set; }
        public Guid PagamentoId { get; set; }
    }
}

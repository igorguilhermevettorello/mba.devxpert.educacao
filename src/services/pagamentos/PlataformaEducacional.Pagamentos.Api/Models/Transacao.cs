using PlataformaEducacional.Core.DomainObjects;

namespace PlataformaEducacional.Pagamentos.Api.Models
{
    public class Transacao : Entity
    {
        public Transacao(string codigoAutorizacao, string bandeiraCartao, DateTime? dataTransacao, decimal valorTotal, decimal custoTransacao, StatusTransacao status, string tID, string nSU, Guid? pagamentoId = null)
        {
            CodigoAutorizacao = codigoAutorizacao;
            BandeiraCartao = bandeiraCartao;
            DataTransacao = dataTransacao;
            ValorTotal = valorTotal;
            CustoTransacao = custoTransacao;
            Status = status;
            TID = tID;
            NSU = nSU;

            if (pagamentoId.HasValue)
                PagamentoId = pagamentoId.Value;
        }

        protected Transacao() { }

        public string CodigoAutorizacao { get; private set; } = string.Empty;
        public string BandeiraCartao { get; private set; } = string.Empty;
        public DateTime? DataTransacao { get; private set; }
        public decimal ValorTotal { get; private set; }
        public decimal CustoTransacao { get; private set; }
        public StatusTransacao Status { get; private set; }
        public string TID { get; private set; } // Id
        public string NSU { get; private set; } // Meio (paypal)
        public Guid PagamentoId { get; private set; }

        // EF Relation
        public Pagamento? Pagamento { get; private set; }

        public void AtualizarPagamentoId(Guid pagamentoId)
        {
            PagamentoId = pagamentoId;
        }
    }
}

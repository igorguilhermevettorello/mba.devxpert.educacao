using PlataformaEducacional.Core.DomainObjects;

namespace PlataformaEducacional.Pedidos.Domain.Vouchers
{
    public class Voucher : Entity, IAggregateRoot
    {
        public string Codigo { get; private set; }
        public decimal? Percentual { get; private set; }
        public decimal? ValorDesconto { get; private set; }
        public int Quantidade { get; private set; }
        public TipoDescontoVoucher TipoDesconto { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataUtilizacao { get; private set; }
        public DateTime DataValidade { get; private set; }
        public bool Ativo { get; private set; }
        public bool Utilizado { get; private set; }

        // Construtor protegido para EF
        protected Voucher() { }

        public static Voucher VoucherFactory(
            string codigo, 
            decimal? percentual, 
            decimal? valorDesconto, 
            int quantidade,
            TipoDescontoVoucher tipoDesconto, 
            DateTime dataValidade)
        {
            return new Voucher
            {
                Codigo = codigo,
                Percentual = percentual,
                ValorDesconto = valorDesconto,
                Quantidade = quantidade,
                TipoDesconto = tipoDesconto,
                DataCriacao = DateTime.Now,
                DataValidade = dataValidade,
                Ativo = true,
                Utilizado = false
            };
        }

        public bool EstaValidoParaUtilizacao()
        {
            return Ativo 
                   && !Utilizado 
                   && Quantidade > 0 
                   && DataValidade >= DateTime.Now;
        }

        public void MarcarComoUtilizado()
        {
            Ativo = false;
            Utilizado = true;
            Quantidade = 0;
            DataUtilizacao = DateTime.Now;
        }

        public void DebitarQuantidade()
        {
            Quantidade -= 1;
            if (Quantidade >= 1) return;

            MarcarComoUtilizado();
        }
    }
}

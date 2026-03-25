using PlataformaEducacional.Core.DomainObjects;

namespace PlataformaEducacional.Pagamentos.Api.Models
{
    public class Pagamento : Entity, IAggregateRoot
    {
        private readonly List<Transacao> _transacoes = new();

        protected Pagamento() { }

        public Pagamento(Guid matriculaId, TipoPagamento tipoPagamento, decimal valor, CartaoCredito cartaoCredito)
        {
            MatriculaId = matriculaId;
            TipoPagamento = tipoPagamento;
            Valor = valor;
            CartaoCredito = cartaoCredito;
        }

        public Guid MatriculaId { get; private set; }
        public TipoPagamento TipoPagamento { get; private set; }
        public decimal Valor { get; set; }

        public CartaoCredito CartaoCredito { get; private set; }

        // EF Relation
        public IReadOnlyCollection<Transacao> Transacoes => _transacoes;

        public void AdicionarTransacao(Transacao transacao)
        {
            _transacoes.Add(transacao);
        }
    }
}

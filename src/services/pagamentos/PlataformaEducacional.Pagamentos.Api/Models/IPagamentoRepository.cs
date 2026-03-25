using PlataformaEducacional.Core.Data;

namespace PlataformaEducacional.Pagamentos.Api.Models
{
    public interface IPagamentoRepository : IRepository<Pagamento>
    {
        void AdicionarPagamento(Pagamento pagamento);
        void AdicionarTransacao(Transacao transacao);
        Task<Pagamento?> ObterPorId(Guid pedidoId);
        Task<Pagamento?> ObterPorMatriculaId(Guid pedidoId);
        Task<IEnumerable<Transacao>?> ObterTransacaoesPorMatriculaId(Guid pedidoId);
    }
}

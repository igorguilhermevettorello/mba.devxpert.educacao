using PlataformaEducacional.Core.Data;

namespace PlataformaEducacional.Pagamentos.Api.Models
{
    public interface IPagamentoRepository : IRepository<Pagamento>
    {
        void AdicionarPagamento(Pagamento pagamento);
        void AdicionarTransacao(Transacao transacao);
        Task<Pagamento> ObterPagamentoPorMatriculaId(Guid pedidoId);
        Task<IEnumerable<Transacao>> ObterTransacaoesPorMatriculaId(Guid pedidoId);
    }
}

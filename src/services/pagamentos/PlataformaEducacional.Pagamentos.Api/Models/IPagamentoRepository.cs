using PlataformaEducacional.Core.Data;

namespace PlataformaEducacional.Pagamentos.Api.Models
{
    public interface IPagamentoRepository : IRepository<Pagamento>
    {
        void AdicionarPagamento(Pagamento pagamento);
        void AdicionarTransacao(Transacao transacao);
        Task<Pagamento?> ObterPorId(Guid id);
        Task<Pagamento?> ObterPorMatriculaId(Guid matriculaId);
        Task<IEnumerable<Transacao>?> ObterTransacaoesPorMatriculaId(Guid matriculaId);
    }
}

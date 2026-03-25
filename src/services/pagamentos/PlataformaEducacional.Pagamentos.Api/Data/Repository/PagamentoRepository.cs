using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Core.Data;
using PlataformaEducacional.Pagamentos.Api.Models;

namespace PlataformaEducacional.Pagamentos.Api.Data.Repository
{
    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly PagamentosContext _context;

        public PagamentoRepository(PagamentosContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public void AdicionarPagamento(Pagamento pagamento)
        {
            _context.Pagamentos.Add(pagamento);
        }

        public void AdicionarTransacao(Transacao transacao)
        {
            _context.Transacoes.Add(transacao);
        }

        public async Task<Pagamento?> ObterPorMatriculaId(Guid matriculaId)
        {
            return await _context.Pagamentos
                .Include(x => x.Transacoes)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.MatriculaId == matriculaId);
        }

        public async Task<Pagamento?> ObterPorId(Guid id)
        {
            return await _context.Pagamentos
                .Include(x => x.Transacoes)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Transacao>?> ObterTransacaoesPorMatriculaId(Guid pedidoId)
        {
            return await _context.Transacoes
                .Include(x => x.Pagamento)
                .AsNoTracking()
                .Where(t => t.Pagamento.MatriculaId == pedidoId).ToListAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

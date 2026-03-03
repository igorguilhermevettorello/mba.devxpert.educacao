using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Core.Data;
using PlataformaEducacional.Pedidos.Domain.Pedidos;
using System.Data.Common;

namespace PlataformaEducacional.Pedidos.Data.Repository
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly PedidosContext _context;

        public PedidoRepository(PedidosContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public DbConnection ObterConexao() => _context.Database.GetDbConnection();

        public async Task<Pedido> ObterPorId(Guid id)
        {
            return await _context.Pedidos.FindAsync(id);
        }

        public async Task<IEnumerable<Pedido>> ObterListaPorClienteId(Guid clienteId)
        {
            return await _context.Pedidos
                .Include(p => p.PedidoItems)
                .AsNoTracking()
                .Where(p => p.ClienteId == clienteId)
                .ToListAsync();
        }

        public void Adicionar(Pedido pedido)
        {
            _context.Pedidos.Add(pedido);
        }

        public void Atualizar(Pedido pedido)
        {
            _context.Pedidos.Update(pedido);
        }

        public async Task<int> ObterProximoCodigo()
        {
            // Supondo que o código do pedido seja um número sequencial armazenado em uma coluna "Codigo"
            var ultimoPedido = await _context.Pedidos
                                             .OrderByDescending(p => p.Codigo)
                                             .FirstOrDefaultAsync();

            return (ultimoPedido != null) ? ultimoPedido.Codigo + 1 : 1000;
        }

        public async Task<PedidoItem> ObterItemPorId(Guid id)
        {
            return await _context.PedidoItems.FindAsync(id);
        }

        public async Task<PedidoItem> ObterItemPorPedido(Guid pedidoId, Guid produtoId)
        {
            return await _context.PedidoItems.FirstOrDefaultAsync(p => p.ProdutoId == produtoId && p.PedidoId == pedidoId);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

using PlataformaEducacional.Core.Data;
using System.Threading.Tasks;

namespace PlataformaEducacional.Pedidos.Domain.Vouchers
{
    public interface IVoucherRepository : IRepository<Voucher>
    {
        Task<Voucher> ObterVoucherPorCodigo(string codigo);
        void Adicionar(Voucher voucher);
        void Atualizar(Voucher voucher);
    }
}

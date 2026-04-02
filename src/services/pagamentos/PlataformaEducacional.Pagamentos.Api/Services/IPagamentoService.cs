using PlataformaEducacional.Core.Messages.Integration;
using PlataformaEducacional.Pagamentos.Api.Models;

namespace PlataformaEducacional.Pagamentos.Api.Services
{
    public interface IPagamentoService
    {
        Task<ResponseMessage> AutorizarPagamento(Pagamento pagamento);
        Task<ResponseMessage> CapturarPagamento(Guid matriculaId);
        Task<ResponseMessage> CancelarPagamento(Guid matriculaId);
    }
}

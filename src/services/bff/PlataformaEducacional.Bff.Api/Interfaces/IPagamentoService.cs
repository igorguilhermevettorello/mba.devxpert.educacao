using PlataformaEducacional.Bff.Api.Models;

namespace PlataformaEducacional.Bff.Api.Interfaces;

public interface IPagamentoService
{
    /// <summary>
    /// Processa um pagamento.
    /// </summary>
    /// <param name="model">Os dados do pagamento a ser processado.</param>
    /// <returns>Retorna true se o pagamento foi processado com sucesso, caso contrário, false.</returns>
    Task<bool> RealizarPagamentoAsync(RealizarPagamentoDto model);
}

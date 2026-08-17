using Microsoft.Extensions.Logging;
using PlataformaEducacional.Bff.Api.Interfaces;
using PlataformaEducacional.Bff.Api.Models;

namespace PlataformaEducacional.Bff.Api.Services
{
    public class PagamentoService : ServiceBase, IPagamentoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PagamentoService> _logger;

        public PagamentoService(HttpClient httpClient, ILogger<PagamentoService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            Logger = logger;
        }

        ///<inheritdoc/>
        public async Task<bool> RealizarPagamentoAsync(RealizarPagamentoDto model)
        {
            _logger.LogInformation("Realizando pagamento para MatriculaId {MatriculaId}, Valor {Valor}", model.MatriculaId, model.ValorCurso);

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/pagamentos", model);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Pagamento realizado com sucesso para MatriculaId {MatriculaId}", model.MatriculaId);
                    return true;
                }

                _logger.LogWarning("Falha ao realizar pagamento para MatriculaId {MatriculaId} - Status {StatusCode}",
                    model.MatriculaId, response.StatusCode);

                TratarErrosResponse(response);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar pagamento para MatriculaId {MatriculaId}", model.MatriculaId);
                throw;
            }
        }

    }
}

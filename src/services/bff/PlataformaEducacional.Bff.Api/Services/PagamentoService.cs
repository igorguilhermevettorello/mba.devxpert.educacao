using PlataformaEducacional.Bff.Api.Interfaces;
using PlataformaEducacional.Bff.Api.Models;

namespace PlataformaEducacional.Bff.Api.Services
{
    public class PagamentoService : ServiceBase, IPagamentoService
    {
        private readonly HttpClient _httpClient;

        public PagamentoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        ///<inheritdoc/>
        public async Task<bool> RealizarPagamentoAsync(RealizarPagamentoDto model)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/pagamentos", model);
            if (response.IsSuccessStatusCode)
                return true;
            TratarErrosResponse(response);
            return false;
        }

    }
}

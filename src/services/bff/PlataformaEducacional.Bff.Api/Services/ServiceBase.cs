using Microsoft.Extensions.Logging;
using PlataformaEducacional.Core.Communication;
using System.Net;
using System.Text;
using System.Text.Json;

namespace PlataformaEducacional.Bff.Api.Services;

public abstract class ServiceBase
{
    protected ILogger Logger { get; set; }

    protected StringContent ObterConteudo(object dado)
    {
        return new StringContent(
            JsonSerializer.Serialize(dado),
            Encoding.UTF8,
            "application/json");
    }

    protected async Task<T> DeserializarObjetoResponse<T>(HttpResponseMessage responseMessage)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        try
        {
            var jsonString = await responseMessage.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(jsonString, options);

            if (result is null
                && responseMessage.StatusCode != HttpStatusCode.Created
                && responseMessage.StatusCode != HttpStatusCode.NoContent
                && responseMessage.StatusCode != HttpStatusCode.OK)
            {
                Logger?.LogError("Desserialização retornou null para response com status {StatusCode}", responseMessage.StatusCode);
                throw new InvalidOperationException("Deserialization returned null.");
            }

            return result;
        }
        catch (JsonException ex)
        {
            Logger?.LogError(ex, "Erro ao desserializar resposta da API - Status {StatusCode}", responseMessage.StatusCode);
            throw;
        }
    }

    protected bool TratarErrosResponse(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.BadRequest) return false;

        try
        {
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (HttpRequestException ex)
        {
            Logger?.LogError(ex, "Erro HTTP na resposta - Status {StatusCode}", response.StatusCode);
            throw;
        }
    }

    protected ResponseResult RetornoOk()
    {
        return new ResponseResult();
    }
}

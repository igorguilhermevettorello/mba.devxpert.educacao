using Microsoft.Extensions.Options;
using PlataformaEducacional.Bff.Api.Extensions;
using PlataformaEducacional.Bff.Api.Interfaces;
using PlataformaEducacional.Bff.Api.Models;
using System.Net;

namespace PlataformaEducacional.Bff.Api.Services;

public class ConteudoService : ServiceBase, IConteudoService
{
    private readonly HttpClient _httpClient;

    public ConteudoService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(settings.Value.ConteudoApiUrl);
    }

    ///<inheritdoc/>
    public async Task<ResultDto<IEnumerable<CursoDto>>> ObterCursoDisponiveisAsync()
    {
        var response = await _httpClient.GetAsync("/api/cursos/ativos");

        if (response.StatusCode == HttpStatusCode.NotFound) return null;

        TratarErrosResponse(response);

        return await DeserializarObjetoResponse<ResultDto<IEnumerable<CursoDto>>>(response);
    }   
}

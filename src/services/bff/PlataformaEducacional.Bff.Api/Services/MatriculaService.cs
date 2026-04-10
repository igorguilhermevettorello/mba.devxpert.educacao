using PlataformaEducacional.Bff.Api.Interfaces;
using PlataformaEducacional.Bff.Api.Models;
using System.Net;

namespace PlataformaEducacional.Bff.Api.Services;

public class MatriculaService : ServiceBase, IMatriculaService
{
    private readonly HttpClient _httpClient;

    public MatriculaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    ///<inheritdoc/>
    public async Task<MatriculaDto> ObterMatriculaPendentesAsync()
    {
        var response = await _httpClient.GetAsync("/pendentes");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return default!;

        TratarErrosResponse(response);

        return await DeserializarObjetoResponse<MatriculaDto>(response);
    }

    ///<inheritdoc/>
    public async Task<MatriculaDto> ObterMatriculaPorId(Guid id)
    {
        var response = await _httpClient.GetAsync($"/matriculas/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return default!;

        TratarErrosResponse(response);

        return await DeserializarObjetoResponse<MatriculaDto>(response);
    }

    ///<inheritdoc/>
    public async Task<MatriculaDto> RealizarMatriculaAsync(RealizarMatriculaDto model)
    {
        var response = await _httpClient.PostAsJsonAsync("/matricula", model);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return default!;

        TratarErrosResponse(response);

        return await DeserializarObjetoResponse<MatriculaDto>(response);
    }
}

using System.Net.Http.Json;
using PlataformaEducacional.Alunos.Application.Services.Models;
using PlataformaEducacional.WebApi.Core.Controllers.Base; // Assumindo que o ResultDto global esteja acessível ou parecido

namespace PlataformaEducacional.Alunos.Application.Services;

public class ConteudoService : IConteudoService
{
    private readonly HttpClient _httpClient;

    public ConteudoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> CursoExisteAsync(Guid cursoId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/cursos/{cursoId}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<Guid?> ObterCursoIdPorAulaAsync(Guid aulaId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/aulas/{aulaId}");
            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<ResultWrapper<AulaDto>>();
            
            return result?.Data?.CursoId;
        }
        catch
        {
            return null;
        }
    }

    public async Task<int> ObterTotalAulasPorCursoAsync(Guid cursoId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/aulas/curso/{cursoId}");
            if (!response.IsSuccessStatusCode)
                return 0;

            var result = await response.Content.ReadFromJsonAsync<ResultWrapper<IEnumerable<AulaDto>>>();
            
            return result?.Data?.Count() ?? 0;
        }
        catch
        {
            return 0;
        }
    }
}

public class ResultWrapper<T>
{
    public T Data { get; set; }
    public bool Success { get; set; }
    public IEnumerable<string> Errors { get; set; }
}

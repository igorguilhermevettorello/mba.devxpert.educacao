using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using PlataformaEducacional.Alunos.Application.Services.Models;

namespace PlataformaEducacional.Alunos.Application.Services;

public class ConteudoService : IConteudoService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ConteudoService> _logger;

    public ConteudoService(HttpClient httpClient, ILogger<ConteudoService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> CursoExisteAsync(Guid cursoId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/cursos/{cursoId}");

            if (!response.IsSuccessStatusCode)
                _logger.LogWarning("Curso {CursoId} não encontrado na API de Conteúdos - Status {StatusCode}", cursoId, response.StatusCode);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao consultar /api/cursos/{CursoId} na API de Conteúdos", cursoId);
            return false;
        }
    }

    public async Task<Guid?> ObterCursoIdPorAulaAsync(Guid aulaId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/aulas/{aulaId}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Aula {AulaId} não encontrada na API de Conteúdos - Status {StatusCode}", aulaId, response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<ResultWrapper<AulaDto>>();

            return result?.Data?.CursoId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao consultar /api/aulas/{AulaId} na API de Conteúdos", aulaId);
            return null;
        }
    }

    public async Task<int> ObterTotalAulasPorCursoAsync(Guid cursoId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/aulas/curso/{cursoId}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Aulas do Curso {CursoId} não encontradas na API de Conteúdos - Status {StatusCode}", cursoId, response.StatusCode);
                return 0;
            }

            var result = await response.Content.ReadFromJsonAsync<ResultWrapper<IEnumerable<AulaDto>>>();

            return result?.Data?.Count() ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao consultar /api/aulas/curso/{CursoId} na API de Conteúdos", cursoId);
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

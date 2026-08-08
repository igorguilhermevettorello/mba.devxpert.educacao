using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlataformaEducacional.Bff.Api.Extensions;
using PlataformaEducacional.Bff.Api.Interfaces;
using PlataformaEducacional.Bff.Api.Models;
using System.Net;

namespace PlataformaEducacional.Bff.Api.Services;

public class ConteudoService : ServiceBase, IConteudoService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ConteudoService> _logger;

    public ConteudoService(HttpClient httpClient, IOptions<AppServicesSettings> settings, ILogger<ConteudoService> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(settings.Value.ConteudoApiUrl);
        _logger = logger;
        Logger = logger;
    }

    ///<inheritdoc/>
    public async Task<ResultDto<IEnumerable<CursoDto>>> ObterCursoDisponiveisAsync()
    {
        _logger.LogInformation("Solicitando cursos disponíveis da API de Conteúdos");

        try
        {
            var response = await _httpClient.GetAsync("/api/cursos/ativos");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Nenhum curso disponível encontrado na API de Conteúdos");
                return null;
            }

            TratarErrosResponse(response);

            var result = await DeserializarObjetoResponse<ResultDto<IEnumerable<CursoDto>>>(response);
            _logger.LogInformation("Cursos disponíveis obtidos com sucesso da API de Conteúdos");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao obter cursos disponíveis da API de Conteúdos");
            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<CursoDto> ObterCursoPorIdAsync(Guid cursoId)
    {
        _logger.LogInformation("Solicitando curso por ID da API de Conteúdos - CursoId {CursoId}", cursoId);

        try
        {
            var response = await _httpClient.GetAsync($"/api/cursos/{cursoId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Curso não encontrado na API de Conteúdos - CursoId {CursoId}", cursoId);
                return null;
            }

            TratarErrosResponse(response);

            var result = await DeserializarObjetoResponse<CursoDto>(response);
            _logger.LogInformation("Curso obtido com sucesso - CursoId {CursoId}", cursoId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao obter curso por ID - CursoId {CursoId}", cursoId);
            throw;
        }
    }
}

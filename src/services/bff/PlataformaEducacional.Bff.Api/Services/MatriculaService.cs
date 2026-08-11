using Microsoft.Extensions.Logging;
using PlataformaEducacional.Bff.Api.Interfaces;
using PlataformaEducacional.Bff.Api.Models;
using System.Net;

namespace PlataformaEducacional.Bff.Api.Services;

public class MatriculaService : ServiceBase, IMatriculaService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MatriculaService> _logger;

    public MatriculaService(HttpClient httpClient, ILogger<MatriculaService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        Logger = logger;
    }

    ///<inheritdoc/>
    public async Task<IEnumerable<MatriculaDto>> ObterMatriculaPendentesAsync(Guid alunoId)
    {
        _logger.LogInformation("Obtendo matrículas pendentes para AlunoId {AlunoId} da API de Alunos", alunoId);

        try
        {
            var response = await _httpClient.GetAsync($"/api/alunos/{alunoId}/matriculas/pendentes");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Nenhuma matrícula pendente encontrada para AlunoId {AlunoId}", alunoId);
                return default!;
            }

            TratarErrosResponse(response);

            _logger.LogInformation("Matrículas pendentes obtidas com sucesso para AlunoId {AlunoId}", alunoId);
            return await DeserializarObjetoResponse<IEnumerable<MatriculaDto>>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao obter matrículas pendentes para AlunoId {AlunoId}", alunoId);
            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<MatriculaDto> ObterMatriculaPorId(Guid id)
    {
        _logger.LogInformation("Obtendo matrícula {MatriculaId} da API de Alunos", id);

        try
        {
            var response = await _httpClient.GetAsync($"/matriculas/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Matrícula {MatriculaId} não encontrada", id);
                return default!;
            }

            TratarErrosResponse(response);

            _logger.LogInformation("Matrícula {MatriculaId} obtida com sucesso", id);
            return await DeserializarObjetoResponse<MatriculaDto>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao obter matrícula {MatriculaId}", id);
            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<MatriculaDto> ObterMatriculaPorIdAsync(Guid id)
    {
        _logger.LogInformation("Obtendo matrícula {MatriculaId} da API de Alunos", id);

        try
        {
            var response = await _httpClient.GetAsync($"/matriculas/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Matrícula {MatriculaId} não encontrada", id);
                return null;
            }

            TratarErrosResponse(response);

            _logger.LogInformation("Matrícula {MatriculaId} obtida com sucesso", id);
            return await DeserializarObjetoResponse<MatriculaDto>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao obter matrícula {MatriculaId}", id);
            return null;
        }
    }

    ///<inheritdoc/>
    public async Task<MatriculaDto> RealizarMatriculaAsync(Guid alunoId, RealizarMatriculaDto model)
    {
        _logger.LogInformation("Realizando matrícula para AlunoId {AlunoId}, CursoId {CursoId}", alunoId, model.CursoId);

        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/alunos/{alunoId}/matriculas", model);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Aluno {AlunoId} ou Curso {CursoId} não encontrado", alunoId, model.CursoId);
                return default!;
            }

            TratarErrosResponse(response);

            _logger.LogInformation("Matrícula realizada com sucesso para AlunoId {AlunoId}, CursoId {CursoId}", alunoId, model.CursoId);
            return await DeserializarObjetoResponse<MatriculaDto>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao realizar matrícula para AlunoId {AlunoId}, CursoId {CursoId}", alunoId, model.CursoId);
            throw;
        }
    }
}

using Microsoft.Extensions.Logging;
using PlataformaEducacional.Pagamentos.Api.Models.DTOs;

namespace PlataformaEducacional.Pagamentos.Api.Services
{
    public class ConteudoService : IConteudoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ConteudoService> _logger;

        public ConteudoService(HttpClient httpClient, ILogger<ConteudoService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<CursoDto?> ObterCursoPorIdAsync(Guid? id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/cursos/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Curso {CursoId} não encontrado na API de Conteúdos - Status {StatusCode}", id, response.StatusCode);
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<ResultWrapper<CursoDto>>();

                return result?.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao consultar /api/cursos/{CursoId} na API de Conteúdos", id);
                return null;
            }
        }
    }

    public class AlunoService : IAlunoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AlunoService> _logger;

        public AlunoService(HttpClient httpClient, ILogger<AlunoService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<MatriculaDto?> ObterMatriculaPorIdAsync(Guid matriculaId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/alunos/matriculas/{matriculaId}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Matrícula {MatriculaId} não encontrada na API de Alunos - Status {StatusCode}", matriculaId, response.StatusCode);
                    return null;
                }

                var matricula = await response.Content.ReadFromJsonAsync<MatriculaDto>();

                return matricula;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao consultar /api/alunos/matriculas/{MatriculaId} na API de Alunos", matriculaId);
                return null;
            }
        }
    }

    public class ResultWrapper<T>
    {
        public T Data { get; set; }
    }
}

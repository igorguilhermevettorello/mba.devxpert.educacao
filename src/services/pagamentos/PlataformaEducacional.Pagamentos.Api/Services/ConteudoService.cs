using PlataformaEducacional.Pagamentos.Api.Models.DTOs;

namespace PlataformaEducacional.Pagamentos.Api.Services
{
    public class ConteudoService : IConteudoService
    {
        private readonly HttpClient _httpClient;

        public ConteudoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CursoDto?> ObterCursoPorIdAsync(Guid? id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/cursos/{id}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var result = await response.Content.ReadFromJsonAsync<ResultWrapper<CursoDto>>();

                return result?.Data;
            }
            catch
            {
                return null;
            }
        }
    }

    public class AlunoService : IAlunoService
    {
        private readonly HttpClient _httpClient;

        public AlunoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<MatriculaDto?> ObterMatriculaPorIdAsync(Guid matriculaId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/alunos/matriculas/{matriculaId}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var matricula = await response.Content.ReadFromJsonAsync<MatriculaDto>();

                return matricula;
            }
            catch
            {
                return null;
            }
        }
    }

    public class ResultWrapper<T>
    {
        public T Data { get; set; }
    }
}

using PlataformaEducacional.Pagamentos.Api.Models.DTOs;

namespace PlataformaEducacional.Pagamentos.Api.Services
{
    public interface IAlunoService
    {
        Task<MatriculaDto?> ObterMatriculaPorIdAsync(Guid matriculaId);
    }
}

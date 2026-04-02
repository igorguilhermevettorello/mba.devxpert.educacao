using PlataformaEducacional.Pagamentos.Api.Models.DTOs;

namespace PlataformaEducacional.Pagamentos.Api.Services
{
    public interface IConteudoService
    {
        Task<CursoDto?> ObterCursoPorIdAsync(Guid? id);
    }
}

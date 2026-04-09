using PlataformaEducacional.Bff.Api.Models;

namespace PlataformaEducacional.Bff.Api.Interfaces;

/// <summary>
/// Interface para o serviço de conteúdo.
/// </summary>
public interface IConteudoService
{
    /// <summary>
    /// Obtém de forma assíncrona os cursos disponíveis.
    /// </summary>
    /// <returns>Uma tarefa que representa a operação assíncrona. O resultado da tarefa contém uma coleção de cursos disponíveis.</returns>
    public Task<IEnumerable<CursoDto>> ObterCursoDisponiveisAsync();

}

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
    public Task<ResultDto<IEnumerable<CursoDto>>> ObterCursoDisponiveisAsync();

    /// <summary>
    /// Obtém de forma assíncrona um curso por ID.
    /// </summary>
    /// <param name="cursoId">ID do curso a ser obtido.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona. O resultado da tarefa contém o curso solicitado ou null se não encontrado.</returns>
    public Task<CursoDto> ObterCursoPorIdAsync(Guid cursoId);
}

using PlataformaEducacional.Bff.Api.Models;

namespace PlataformaEducacional.Bff.Api.Interfaces;

public interface IMatriculaService
{
    /// <summary>
    /// Obtém de forma assíncrona as matrículas pendentes do aluno.
    /// </summary>
    /// <returns>Uma tarefa que representa a operação assíncrona. O resultado da tarefa contém as matrículas pendentes do aluno.</returns>
    public Task<MatriculaDto> ObterMatriculaPendentesAsync();

    /// <summary>
    /// Obtém de forma assíncrona uma matrícula pelo seu identificador.
    /// </summary>
    /// <param name="id">O identificador da matrícula.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona. O resultado da tarefa contém a matrícula correspondente ao identificador fornecido.</returns>
    public Task<MatriculaDto> ObterMatriculaPorId(Guid id);

    /// <summary>
    /// Realiza de forma assíncrona a matrícula em um curso.
    /// </summary>
    /// <param name="model">O modelo contendo os dados da matrícula.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona. O resultado da tarefa contém a matrícula realizada.</returns>
    public Task<MatriculaDto> RealizarMatriculaAsync(RealizarMatriculaDto model);
}

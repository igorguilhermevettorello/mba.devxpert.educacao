using PlataformaEducacional.Core.Data;

namespace PlataformaEducacional.Alunos.Api.Models;

public interface IAlunoRepository : IRepository<Aluno>
{
    /// <summary>
    /// Adds a student to the collection.
    /// </summary>
    /// <param name="aluno">The student to add.</param>
    void Adicionar(Aluno aluno);

    /// <summary>
    /// Adiciona um endereço a um aluno existente.
    /// </summary>
    /// <param name="endereco">O endereço a ser adicionado.</param>
    void AdicionarEndereco(Endereco endereco);

    /// <summary>
    /// Obtém um aluno pelo seu ID.
    /// </summary>
    /// <param name="id">ID do aluno.</param>
    /// <returns>O aluno correspondente ao ID fornecido, ou null se não encontrado.</returns>
    Task<Aluno?> ObterPorId(Guid id);

    /// <summary>
    /// Obtém o endereço de um aluno pelo ID do aluno.
    /// </summary>
    /// <param name="id">ID do aluno.</param>
    /// <returns>O endereço correspondente ao ID do aluno fornecido, ou null se não encontrado.</returns>
    Task<Endereco?> ObterEnderecoPorAlunoId(Guid id);

    /// <summary>
    /// Obtém um aluno pelo seu CPF.
    /// </summary>
    /// <param name="cpf">CPF do aluno.</param>
    /// <returns>O aluno correspondente ao CPF fornecido, ou null se não encontrado.</returns>
    Task<Aluno?> ObterPorCpf(string cpf);

    /// <summary>
    /// Obtém todos os alunos.
    /// </summary>
    /// <returns>Uma lista de todos os alunos.</returns>
    Task<IEnumerable<Aluno>> ObterTodos();
}

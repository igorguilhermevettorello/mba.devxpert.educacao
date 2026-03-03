using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Core.Data;

namespace PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories
{
    public interface ICursoRepository : IRepository<Curso>
    {
        void Adicionar(Curso curso);
        void Alterar(Curso curso);
        void Remover(Curso curso);
        Task<Curso> BuscarPorIdAsync(Guid id);
        Task<IEnumerable<Curso>> ObterTodosAsync();
        Task<IEnumerable<Curso>> ObterAtivosAsync();
    }

}

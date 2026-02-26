using PlataformaEducacional.Conteudo.Domain.Entities;

namespace PlataformaEducacional.Conteudo.Domain.Interfaces.Services
{
    public interface ICursoService : IDisposable
    {
        Task<bool> VerificarAulas(Guid cursoId, Aula aula);
    }
}

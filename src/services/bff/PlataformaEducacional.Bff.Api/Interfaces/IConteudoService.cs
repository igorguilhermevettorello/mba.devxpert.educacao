using PlataformaEducacional.Bff.Api.Models;

namespace PlataformaEducacional.Bff.Api.Interfaces;

public interface IConteudoService
{
    public Task<IEnumerable<CursoDto>> ObterCursoDisponiveisAsync();

}

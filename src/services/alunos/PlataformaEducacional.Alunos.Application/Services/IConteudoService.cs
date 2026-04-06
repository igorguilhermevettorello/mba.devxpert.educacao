namespace PlataformaEducacional.Alunos.Application.Services;

public interface IConteudoService
{
    Task<bool> CursoExisteAsync(Guid cursoId);
    Task<Guid?> ObterCursoIdPorAulaAsync(Guid aulaId);
    Task<int> ObterTotalAulasPorCursoAsync(Guid cursoId);
}

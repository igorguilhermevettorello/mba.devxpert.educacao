using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Services;
using PlataformaEducacional.Core.Mediator;

namespace PlataformaEducacional.Conteudo.Domain.Services
{
    public class CursoService : ICursoService
    {
        private readonly ICursoRepository _cursoRepository;
        private readonly IMediatorHandler _mediatorHandler;

        public CursoService(ICursoRepository cursoRepository, IMediatorHandler mediatorHandler)
        {
            _cursoRepository = cursoRepository;
            _mediatorHandler = mediatorHandler;
        }

        public async Task<bool> VerificarAulas(Guid cursoId, Aula aula)
        {
            var curso = await _cursoRepository.BuscarPorIdAsync(cursoId);

            var teste = curso.Aulas.FirstOrDefault(p => p.Descricao.Equals(aula.Descricao));

            if (teste != null) return false;

            //await _mediatorHandler.PublicarEvento(new CursoAvisarAlunosEvent(cursoId, aula.Descricao));

            return true;
        }

        public void Dispose()
        {
            _cursoRepository.Dispose();
        }
    }
}

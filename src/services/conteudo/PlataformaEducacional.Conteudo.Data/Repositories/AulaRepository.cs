using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Conteudo.Data.Context;
using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Core.Data;

namespace PlataformaEducacional.Conteudo.Data.Repositories
{
    public class AulaRepository : IAulaRepository
    {
        private readonly CursoContext _context;

        public AulaRepository(CursoContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public void Adicionar(Aula aula)
        {
            _context.Aulas.Add(aula);
        }

        public void Alterar(Aula aula)
        {
            _context.Aulas.Update(aula);
        }

        public void Remover(Aula aula)
        {
            _context.Aulas.Remove(aula);
        }

        public async Task<Aula?> BuscarPorIdAsync(Guid id)
        {
            return await _context.Aulas
                .Include(a => a.Curso)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Aula>> ObterTodasAsync()
        {
            return await _context.Aulas
                .AsNoTracking()
                .OrderBy(a => a.Ordem)
                .ToListAsync();
        }

        public async Task<IEnumerable<Aula>> ObterAtivasAsync()
        {
            return await _context.Aulas
                .Where(a => a.Ativa)
                .AsNoTracking()
                .OrderBy(a => a.Ordem)
                .ToListAsync();
        }

        public async Task<IEnumerable<Aula>> ObterPorCursoIdAsync(Guid cursoId)
        {
            return await _context.Aulas
                .Where(a => a.CursoId == cursoId)
                .AsNoTracking()
                .OrderBy(a => a.Ordem)
                .ToListAsync();
        }

        public async Task<IEnumerable<Aula>> ObterAtivasPorCursoIdAsync(Guid cursoId)
        {
            return await _context.Aulas
                .Where(a => a.CursoId == cursoId && a.Ativa)
                .AsNoTracking()
                .OrderBy(a => a.Ordem)
                .ToListAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}

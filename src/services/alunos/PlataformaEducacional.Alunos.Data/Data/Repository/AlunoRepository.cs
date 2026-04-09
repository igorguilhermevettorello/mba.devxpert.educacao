using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Alunos.Domain.Interfaces;
using PlataformaEducacional.Alunos.Domain.Models;
using PlataformaEducacional.Core.Data;
using PlataformaEducacional.WebApi.Core.Enumerators;

namespace PlataformaEducacional.Alunos.Data.Repository;

public class AlunoRepository : IAlunoRepository
{
    private readonly AlunosContext _context;

    public AlunoRepository(AlunosContext context)
    {
        _context = context;
    }

    ///<inheritdoc/>
    public IUnitOfWork UnitOfWork => _context;

    ///<inheritdoc/>
    public async Task<Aluno?> ObterPorId(Guid id)
    {
        return await _context.Alunos
            .Include(x => x.Matriculas)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    ///<inheritdoc/>
    public async Task<Endereco?> ObterEnderecoPorAlunoId(Guid id)
    {
        return await _context.Enderecos.FirstOrDefaultAsync(a => a.AlunoId == id);
    }

    ///<inheritdoc/>
    public async Task<Aluno?> ObterPorCpf(string cpf)
    {
        return await _context.Alunos.FirstOrDefaultAsync(a => a.Cpf.Numero == cpf);
    }

    ///<inheritdoc/>
    public async Task<IEnumerable<Aluno>> ObterTodos()
    {
        return await _context.Alunos.AsNoTracking().ToListAsync();
    }

    ///<inheritdoc/>
    public void Adicionar(Aluno aluno)
    {
        _context.Alunos.Add(aluno);
    }

    ///<inheritdoc/>
    public void AdicionarEndereco(Endereco endereco)
    {
        _context.Enderecos.Add(endereco);
    }

    ///<inheritdoc/>
    public void AdicionarMatricula(Matricula matricula)
    {
        _context.Matriculas.Add(matricula);
    }

    ///<inheritdoc/>
    public void AtualizarMatricula(Matricula matricula)
    {
        _context.Matriculas.Update(matricula);
    }

    ///<inheritdoc/>
    public async Task<Matricula?> ObterMatriculaPorId(Guid id)
    {
        return await _context.Matriculas
           .Include(m => m.Aluno)
           .Include(m => m.ProgressoAulas)
           .Include(m => m.Certificado)
           .FirstOrDefaultAsync(m => m.Id == id);
    }

    ///<inheritdoc/>
    public async Task<Matricula?> ObterMatriculaPendentes()
    {
        return await _context.Matriculas.AsNoTracking()
                                        .FirstOrDefaultAsync(m => m.Status == StatusMatricula.Pendente);
    }

    ///<inheritdoc/>
    public async Task<IEnumerable<Matricula>> ObterMatriculasPorAluno(Guid alunoId)
    {
        return await _context.Matriculas
            .Include(m => m.ProgressoAulas)
            .Include(m => m.Certificado)
            .AsNoTracking()
            .Where(m => m.AlunoId == alunoId)
            .ToListAsync();
    }

    ///<inheritdoc/>
    public void AdicionarProgresso(ProgressoAula progressoAula)
    {
        _context.ProgressoAulas.Add(progressoAula);
    }

    ///<inheritdoc/>
    public async Task<Certificado?> ObterCertificado(Guid id)
    {
        return await _context.Certificados.FirstOrDefaultAsync(c => c.Id == id);
    }

    
    public void Dispose()
    {
        _context.Dispose();
    }
}

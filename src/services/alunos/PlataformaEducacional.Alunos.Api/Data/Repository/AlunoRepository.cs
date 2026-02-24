using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Alunos.Api.Interfaces;
using PlataformaEducacional.Alunos.Api.Models;
using PlataformaEducacional.Core.Data;

namespace PlataformaEducacional.Alunos.Api.Data.Repository;

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
        return await _context.Alunos.FirstOrDefaultAsync(a => a.Id == id);
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
    public void AdicionarEndereco(Endereco endereco)
    {
        _context.Enderecos.Add(endereco);
    }
    
    ///<inheritdoc/>
    public void Dispose()
    {
        _context.Dispose();
    }

}

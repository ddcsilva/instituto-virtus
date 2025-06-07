using Microsoft.EntityFrameworkCore;
using Virtus.Domain.Entities;
using Virtus.Domain.Enums;
using Virtus.Domain.Interfaces;
using Virtus.Infrastructure.Data;

namespace Virtus.Infrastructure.Repositories;

public class MatriculaRepository : BaseRepository<Matricula>, IMatriculaRepository
{
  public MatriculaRepository(VirtusDbContext context) : base(context) { }

  public override async Task<Matricula?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(m => m.Aluno)
        .ThenInclude(a => a.Pessoa)
      .Include(m => m.Turma)
        .ThenInclude(t => t.Professor)
          .ThenInclude(p => p.Pessoa)
      .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
  }

  public async Task<IEnumerable<Matricula>> ObterMatriculasPorAlunoAsync(int alunoId, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(m => m.Turma)
        .ThenInclude(t => t.Professor)
          .ThenInclude(p => p.Pessoa)
      .Where(m => m.AlunoId == alunoId)
      .OrderByDescending(m => m.DataMatricula)
      .ToListAsync(cancellationToken);
  }

  public async Task<IEnumerable<Matricula>> ObterMatriculasPorTurmaAsync(int turmaId, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(m => m.Aluno)
        .ThenInclude(a => a.Pessoa)
      .Where(m => m.TurmaId == turmaId)
      .OrderBy(m => m.NumeroOrdemEspera)
      .ThenBy(m => m.Aluno.Pessoa.Nome)
      .ToListAsync(cancellationToken);
  }

  public async Task<IEnumerable<Matricula>> ObterMatriculasAtivasAsync(CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(m => m.Aluno)
        .ThenInclude(a => a.Pessoa)
      .Include(m => m.Turma)
      .Where(m => m.Status == StatusMatricula.Ativa)
      .OrderBy(m => m.Turma.Nome)
      .ThenBy(m => m.Aluno.Pessoa.Nome)
      .ToListAsync(cancellationToken);
  }

  public async Task<IEnumerable<Matricula>> ObterMatriculasPorStatusAsync(StatusMatricula status, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(m => m.Aluno)
        .ThenInclude(a => a.Pessoa)
      .Include(m => m.Turma)
      .Where(m => m.Status == status)
      .OrderByDescending(m => m.DataMatricula)
      .ToListAsync(cancellationToken);
  }

  public async Task<Matricula?> ObterMatriculaAtivaAsync(int alunoId, int turmaId, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .FirstOrDefaultAsync(m =>
        m.AlunoId == alunoId &&
        m.TurmaId == turmaId &&
        m.Status == StatusMatricula.Ativa,
        cancellationToken);
  }

  public async Task<int> ObterProximoNumeroListaEsperaAsync(int turmaId, CancellationToken cancellationToken = default)
  {
    var ultimoNumero = await _dbSet
      .Where(m => m.TurmaId == turmaId && m.NumeroOrdemEspera > 0)
      .MaxAsync(m => (int?)m.NumeroOrdemEspera, cancellationToken) ?? 0;

    return ultimoNumero + 1;
  }
}
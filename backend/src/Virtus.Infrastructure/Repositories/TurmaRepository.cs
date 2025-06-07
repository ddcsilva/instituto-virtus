using Microsoft.EntityFrameworkCore;
using Virtus.Domain.Entities;
using Virtus.Domain.Enums;
using Virtus.Domain.Interfaces;
using Virtus.Infrastructure.Data;

namespace Virtus.Infrastructure.Repositories;

public class TurmaRepository : BaseRepository<Turma>, ITurmaRepository
{
  public TurmaRepository(VirtusDbContext context) : base(context) { }

  public override async Task<Turma?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(t => t.Professor)
        .ThenInclude(p => p.Pessoa)
      .Include(t => t.Matriculas)
        .ThenInclude(m => m.Aluno)
          .ThenInclude(a => a.Pessoa)
      .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
  }

  public async Task<IEnumerable<Turma>> ObterTurmasAtivasAsync(CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(t => t.Professor)
        .ThenInclude(p => p.Pessoa)
      .Where(t => t.Ativa)
      .OrderBy(t => t.Nome)
      .ToListAsync(cancellationToken);
  }

  public async Task<IEnumerable<Turma>> ObterTurmasPorTipoAsync(TipoCurso tipo, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(t => t.Professor)
        .ThenInclude(p => p.Pessoa)
      .Where(t => t.Tipo == tipo)
      .OrderBy(t => t.Nome)
      .ToListAsync(cancellationToken);
  }

  public async Task<IEnumerable<Turma>> ObterTurmasPorProfessorAsync(int professorId, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(t => t.Matriculas)
      .Where(t => t.ProfessorId == professorId)
      .OrderBy(t => t.DiaSemana)
      .ThenBy(t => t.Horario)
      .ToListAsync(cancellationToken);
  }

  public async Task<IEnumerable<Turma>> ObterTurmasComVagasAsync(CancellationToken cancellationToken = default)
  {
    var turmasComVagas = await _dbSet
      .Include(t => t.Professor)
        .ThenInclude(p => p.Pessoa)
      .Include(t => t.Matriculas)
      .Where(t => t.Ativa)
      .ToListAsync(cancellationToken);

    // Filtra no cliente para usar a lógica de negócio
    return turmasComVagas
      .Where(t => t.TemVagasDisponiveis())
      .OrderBy(t => t.Nome);
  }

  public async Task<int> ObterQuantidadeAlunosMatriculadosAsync(int turmaId, CancellationToken cancellationToken = default)
  {
    return await _context.Matriculas
      .CountAsync(m => m.TurmaId == turmaId && m.Status == StatusMatricula.Ativa && m.NumeroOrdemEspera == 0, cancellationToken);
  }
}
using Microsoft.EntityFrameworkCore;
using Virtus.Domain.Entities;
using Virtus.Domain.Enums;
using Virtus.Domain.Interfaces;
using Virtus.Infrastructure.Data;

namespace Virtus.Infrastructure.Repositories;

public class ProfessorRepository : BaseRepository<Professor>, IProfessorRepository
{
  public ProfessorRepository(VirtusDbContext context) : base(context) { }

  public override async Task<Professor?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(p => p.Pessoa)
      .Include(p => p.Turmas)
      .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
  }

  public async Task<Professor?> ObterPorPessoaIdAsync(int pessoaId, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(p => p.Pessoa)
      .FirstOrDefaultAsync(p => p.PessoaId == pessoaId, cancellationToken);
  }

  public async Task<IEnumerable<Professor>> ObterProfessoresAtivosAsync(CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(p => p.Pessoa)
      .Where(p => p.Ativo)
      .OrderBy(p => p.Pessoa.Nome)
      .ToListAsync(cancellationToken);
  }

  public async Task<IEnumerable<Professor>> ObterProfessoresPorEspecialidadeAsync(string especialidade, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(p => p.Pessoa)
      .Where(p => p.Especialidade.Contains(especialidade))
      .OrderBy(p => p.Pessoa.Nome)
      .ToListAsync(cancellationToken);
  }

  public async Task<IEnumerable<Professor>> ObterProfessoresQuePodeLecionarAsync(TipoCurso tipoCurso, CancellationToken cancellationToken = default)
  {
    var query = _dbSet
      .Include(p => p.Pessoa)
      .Where(p => p.Ativo);

    // Filtra por tipo de curso baseado na especialidade
    query = tipoCurso switch
    {
      TipoCurso.Violao => query.Where(p => p.Especialidade.Contains("Violão")),
      TipoCurso.Teclado => query.Where(p => p.Especialidade.Contains("Teclado") || p.Especialidade.Contains("Piano")),
      TipoCurso.Canto => query.Where(p => p.Especialidade.Contains("Canto") || p.Especialidade.Contains("Vocal")),
      TipoCurso.Teologia => query.Where(p => p.Especialidade.Contains("Teologia")),
      _ => query
    };

    return await query
      .OrderBy(p => p.Pessoa.Nome)
      .ToListAsync(cancellationToken);
  }
}
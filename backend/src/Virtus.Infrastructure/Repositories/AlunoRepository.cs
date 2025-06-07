using Microsoft.EntityFrameworkCore;
using Virtus.Domain.Entities;
using Virtus.Domain.Enums;
using Virtus.Domain.Interfaces;
using Virtus.Infrastructure.Data;

namespace Virtus.Infrastructure.Repositories;

public class AlunoRepository : BaseRepository<Aluno>, IAlunoRepository
{
  public AlunoRepository(VirtusDbContext context) : base(context) { }

  public override async Task<Aluno?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(a => a.Pessoa)
      .Include(a => a.Responsavel)
      .Include(a => a.Matriculas)
        .ThenInclude(m => m.Turma)
      .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
  }

  public async Task<Aluno?> ObterPorPessoaIdAsync(int pessoaId, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(a => a.Pessoa)
      .Include(a => a.Responsavel)
      .FirstOrDefaultAsync(a => a.PessoaId == pessoaId, cancellationToken);
  }

  public async Task<IEnumerable<Aluno>> ObterPorStatusAsync(StatusAluno status, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(a => a.Pessoa)
      .Where(a => a.Status == status)
      .OrderBy(a => a.Pessoa.Nome)
      .ToListAsync(cancellationToken);
  }

  public async Task<IEnumerable<Aluno>> ObterPorResponsavelIdAsync(int responsavelId, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(a => a.Pessoa)
      .Where(a => a.ResponsavelId == responsavelId)
      .OrderBy(a => a.Pessoa.Nome)
      .ToListAsync(cancellationToken);
  }

  public async Task<IEnumerable<Aluno>> ObterAlunosComMatriculasAtivasAsync(CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(a => a.Pessoa)
      .Include(a => a.Matriculas)
        .ThenInclude(m => m.Turma)
      .Where(a => a.Matriculas.Any(m => m.Status == StatusMatricula.Ativa))
      .OrderBy(a => a.Pessoa.Nome)
      .ToListAsync(cancellationToken);
  }

  public async Task<IEnumerable<Aluno>> ObterAlunosEmListaEsperaAsync(int turmaId, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(a => a.Pessoa)
      .Include(a => a.Matriculas)
      .Where(a => a.Matriculas.Any(m =>
        m.TurmaId == turmaId &&
        m.Status == StatusMatricula.Ativa &&
        m.NumeroOrdemEspera > 0))
      .OrderBy(a => a.Matriculas.First(m => m.TurmaId == turmaId).NumeroOrdemEspera)
      .ToListAsync(cancellationToken);
  }
}
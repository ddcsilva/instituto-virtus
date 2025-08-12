using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using InstitutoVirtus.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoVirtus.Infrastructure.Data.Repositories;

public class CursoRepository : BaseRepository<Curso>, ICursoRepository
{
  public CursoRepository(VirtusDbContext context) : base(context) { }

  public async Task<IEnumerable<Curso>> GetAtivosAsync(CancellationToken cancellationToken = default)
  {
    return await _context.Cursos
        .Include(c => c.Turmas)
        .Where(c => c.Ativo)
        .OrderBy(c => c.Nome)
        .ToListAsync(cancellationToken);
  }

  public async Task<Curso?> GetByNomeAsync(string nome, CancellationToken cancellationToken = default)
  {
    return await _context.Cursos
        .FirstOrDefaultAsync(c => c.Nome == nome, cancellationToken);
  }

  public override async Task<Curso?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    return await _context.Cursos
        .Include(c => c.Turmas)
            .ThenInclude(t => t.Professor)
        .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
  }

  public async Task<(IEnumerable<Curso> Items, int TotalCount)> SearchAsync(
    string? nome,
    bool? ativo,
    int page,
    int pageSize,
    CancellationToken cancellationToken = default)
  {
    var query = _context.Cursos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(nome)) query = query.Where(c => c.Nome.Contains(nome));
    if (ativo.HasValue) query = query.Where(c => c.Ativo == ativo.Value);

    var total = await query.CountAsync(cancellationToken);
    var items = await query
      .OrderBy(c => c.Nome)
      .Skip(page * pageSize)
      .Take(pageSize)
      .ToListAsync(cancellationToken);
    return (items, total);
  }
}

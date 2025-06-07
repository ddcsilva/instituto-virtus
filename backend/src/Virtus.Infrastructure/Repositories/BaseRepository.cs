using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Virtus.Domain.Entities;
using Virtus.Domain.Interfaces;
using Virtus.Infrastructure.Data;

namespace Virtus.Infrastructure.Repositories;

public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
  protected readonly VirtusDbContext _context;
  protected readonly DbSet<TEntity> _dbSet;

  public BaseRepository(VirtusDbContext context)
  {
    _context = context;
    _dbSet = context.Set<TEntity>();
  }

  public virtual async Task<TEntity?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
  {
    return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
  }

  public virtual async Task<IEnumerable<TEntity>> ObterTodosAsync(CancellationToken cancellationToken = default)
  {
    return await _dbSet.ToListAsync(cancellationToken);
  }

  public virtual async Task<IEnumerable<TEntity>> BuscarAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
  {
    return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
  }

  public virtual async Task<TEntity?> ObterPrimeiroAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
  {
    return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
  }

  public virtual async Task<bool> ExisteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
  {
    return await _dbSet.AnyAsync(predicate, cancellationToken);
  }

  public virtual async Task<int> ContarAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
  {
    if (predicate == null)
      return await _dbSet.CountAsync(cancellationToken);

    return await _dbSet.CountAsync(predicate, cancellationToken);
  }

  public virtual async Task AdicionarAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    await _dbSet.AddAsync(entity, cancellationToken);
  }

  public virtual async Task AdicionarRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
  {
    await _dbSet.AddRangeAsync(entities, cancellationToken);
  }

  public virtual void Atualizar(TEntity entity)
  {
    _dbSet.Update(entity);
  }

  public virtual void AtualizarRange(IEnumerable<TEntity> entities)
  {
    _dbSet.UpdateRange(entities);
  }

  public virtual void Remover(TEntity entity)
  {
    _dbSet.Remove(entity);
  }

  public virtual void RemoverRange(IEnumerable<TEntity> entities)
  {
    _dbSet.RemoveRange(entities);
  }
}
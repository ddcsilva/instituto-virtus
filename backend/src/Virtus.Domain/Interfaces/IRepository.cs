using System.Linq.Expressions;
using Virtus.Domain.Entities;

namespace Virtus.Domain.Interfaces;

/// <summary>
/// Interface base para todos os repositórios
/// </summary>
public interface IRepository<TEntity> where TEntity : BaseEntity
{
  Task<TEntity?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
  Task<IEnumerable<TEntity>> ObterTodosAsync(CancellationToken cancellationToken = default);
  Task<IEnumerable<TEntity>> BuscarAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
  Task<TEntity?> ObterPrimeiroAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
  Task<bool> ExisteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
  Task<int> ContarAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

  Task AdicionarAsync(TEntity entity, CancellationToken cancellationToken = default);
  Task AdicionarRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

  void Atualizar(TEntity entity);
  void AtualizarRange(IEnumerable<TEntity> entities);

  void Remover(TEntity entity);
  void RemoverRange(IEnumerable<TEntity> entities);
}
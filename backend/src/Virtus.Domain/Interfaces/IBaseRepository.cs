using System.Linq.Expressions;
using Virtus.Domain.Entities;

namespace Virtus.Domain.Interfaces;

/// <summary>
/// Interface base para repositórios.
/// </summary>
/// <typeparam name="TEntidade">O tipo da entidade.</typeparam>
public interface IBaseRepository<TEntidade> where TEntidade : BaseEntity
{
    public Task<TEntidade?> ObterPorIdAsync(int id);
    public Task<IEnumerable<TEntidade>> ObterTodosAsync();
    public Task<IEnumerable<TEntidade>> BuscarAsync(Expression<Func<TEntidade, bool>> predicado);
    public Task<TEntidade> AdicionarAsync(TEntidade entidade);
    public Task AtualizarAsync(TEntidade entidade);
    public Task RemoverAsync(TEntidade entidade);
    public Task<int> ContarAsync(Expression<Func<TEntidade, bool>>? predicado = null);
    public Task<bool> ExisteAsync(Expression<Func<TEntidade, bool>> predicado);
}

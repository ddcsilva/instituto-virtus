using InstitutoVirtus.Domain.Entities;

namespace InstitutoVirtus.Domain.Interfaces.Repositories;

public interface ICursoRepository : IBaseRepository<Curso>
{
    Task<IEnumerable<Curso>> GetAtivosAsync(CancellationToken cancellationToken = default);
    Task<Curso?> GetByNomeAsync(string nome, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Curso> Items, int TotalCount)> SearchAsync(
        string? nome,
        bool? ativo,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
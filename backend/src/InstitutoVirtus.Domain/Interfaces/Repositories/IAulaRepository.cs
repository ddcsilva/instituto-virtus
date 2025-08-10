using InstitutoVirtus.Domain.Entities;

namespace InstitutoVirtus.Domain.Interfaces.Repositories;

public interface IAulaRepository : IBaseRepository<Aula>
{
    Task<IEnumerable<Aula>> GetByTurmaAsync(Guid turmaId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Aula>> GetByPeriodoAsync(DateTime inicio, DateTime fim, CancellationToken cancellationToken = default);
    Task<Aula?> GetByTurmaAndDataAsync(Guid turmaId, DateTime data, CancellationToken cancellationToken = default);
}
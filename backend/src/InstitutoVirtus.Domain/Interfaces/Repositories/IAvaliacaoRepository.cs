using InstitutoVirtus.Domain.Entities;

namespace InstitutoVirtus.Domain.Interfaces.Repositories;

public interface IAvaliacaoRepository : IBaseRepository<Avaliacao>
{
    Task<IEnumerable<Avaliacao>> GetByTurmaAsync(Guid turmaId, CancellationToken cancellationToken = default);
    Task<decimal> CalcularMediaFinalAsync(Guid alunoId, Guid turmaId, CancellationToken cancellationToken = default);
    Task<double> CalcularFrequenciaAsync(Guid alunoId, Guid turmaId, CancellationToken cancellationToken = default);
}
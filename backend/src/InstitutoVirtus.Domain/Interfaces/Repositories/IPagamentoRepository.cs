using InstitutoVirtus.Domain.Entities;

namespace InstitutoVirtus.Domain.Interfaces.Repositories;

public interface IPagamentoRepository : IBaseRepository<Pagamento>
{
    Task<IEnumerable<Pagamento>> GetByResponsavelAsync(Guid responsavelId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Pagamento>> GetByPeriodoAsync(DateTime inicio, DateTime fim, CancellationToken cancellationToken = default);
    Task<Pagamento?> GetByReferenciaExternaAsync(string referencia, CancellationToken cancellationToken = default);
}
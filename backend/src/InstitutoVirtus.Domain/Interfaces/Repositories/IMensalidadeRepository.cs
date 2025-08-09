using InstitutoVirtus.Domain.Entities;

namespace InstitutoVirtus.Domain.Interfaces.Repositories;

public interface IMensalidadeRepository : IBaseRepository<Mensalidade>
{
    Task<IEnumerable<Mensalidade>> GetByMatriculaAsync(Guid matriculaId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Mensalidade>> GetByCompetenciaAsync(int ano, int mes, CancellationToken cancellationToken = default);
    Task<IEnumerable<Mensalidade>> GetVencidasAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Mensalidade>> GetEmAbertoByResponsavelAsync(Guid responsavelId, CancellationToken cancellationToken = default);
}
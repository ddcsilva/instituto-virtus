using InstitutoVirtus.Domain.Entities;

namespace InstitutoVirtus.Domain.Interfaces.Repositories;

public interface IMatriculaRepository : IBaseRepository<Matricula>
{
    Task<IEnumerable<Matricula>> GetByAlunoAsync(Guid alunoId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Matricula>> GetByTurmaAsync(Guid turmaId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Matricula>> GetAtivasAsync(CancellationToken cancellationToken = default);
    Task<bool> AlunoJaMatriculadoNaTurmaAsync(Guid alunoId, Guid turmaId, CancellationToken cancellationToken = default);
}
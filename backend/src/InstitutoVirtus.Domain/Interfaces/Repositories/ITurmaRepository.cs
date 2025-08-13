using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Enums;

namespace InstitutoVirtus.Domain.Interfaces.Repositories;

public interface ITurmaRepository : IBaseRepository<Turma>
{
    Task<IEnumerable<Turma>> GetByCursoAsync(Guid cursoId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Turma>> GetByProfessorAsync(Guid professorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Turma>> GetByPeriodoAsync(int ano, int periodo, CancellationToken cancellationToken = default);
    Task<IEnumerable<Turma>> GetByAnoLetivoAsync(int ano, CancellationToken cancellationToken = default);
    Task<IEnumerable<Turma>> GetComVagasAsync(CancellationToken cancellationToken = default);
    Task<bool> ExisteConflitoHorarioAsync(Guid professorId, DiaSemana dia, TimeSpan horaInicio, CancellationToken cancellationToken = default);
}
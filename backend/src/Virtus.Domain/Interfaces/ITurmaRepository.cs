using Virtus.Domain.Entities;
using Virtus.Domain.Enums;

namespace Virtus.Domain.Interfaces;

/// <summary>
/// Interface específica para o repositório de turmas
/// </summary>
public interface ITurmaRepository : IRepository<Turma>
{
  Task<IEnumerable<Turma>> ObterTurmasAtivasAsync(CancellationToken cancellationToken = default);
  Task<IEnumerable<Turma>> ObterTurmasPorTipoAsync(TipoCurso tipo, CancellationToken cancellationToken = default);
  Task<IEnumerable<Turma>> ObterTurmasPorProfessorAsync(int professorId, CancellationToken cancellationToken = default);
  Task<IEnumerable<Turma>> ObterTurmasComVagasAsync(CancellationToken cancellationToken = default);
  Task<int> ObterQuantidadeAlunosMatriculadosAsync(int turmaId, CancellationToken cancellationToken = default);
}
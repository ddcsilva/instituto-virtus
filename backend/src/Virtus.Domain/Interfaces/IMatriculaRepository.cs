using Virtus.Domain.Entities;
using Virtus.Domain.Enums;

namespace Virtus.Domain.Interfaces;

/// <summary>
/// Interface específica para o repositório de matrículas
/// </summary>
public interface IMatriculaRepository : IRepository<Matricula>
{
  Task<IEnumerable<Matricula>> ObterMatriculasPorAlunoAsync(int alunoId, CancellationToken cancellationToken = default);
  Task<IEnumerable<Matricula>> ObterMatriculasPorTurmaAsync(int turmaId, CancellationToken cancellationToken = default);
  Task<IEnumerable<Matricula>> ObterMatriculasAtivasAsync(CancellationToken cancellationToken = default);
  Task<IEnumerable<Matricula>> ObterMatriculasPorStatusAsync(StatusMatricula status, CancellationToken cancellationToken = default);
  Task<Matricula?> ObterMatriculaAtivaAsync(int alunoId, int turmaId, CancellationToken cancellationToken = default);
  Task<int> ObterProximoNumeroListaEsperaAsync(int turmaId, CancellationToken cancellationToken = default);
}
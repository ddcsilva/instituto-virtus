using Virtus.Domain.Entities;
using Virtus.Domain.Enums;

namespace Virtus.Domain.Interfaces;

/// <summary>
/// Interface específica para o repositório de professores
/// </summary>
public interface IProfessorRepository : IRepository<Professor>
{
  Task<Professor?> ObterPorPessoaIdAsync(int pessoaId, CancellationToken cancellationToken = default);
  Task<IEnumerable<Professor>> ObterProfessoresAtivosAsync(CancellationToken cancellationToken = default);
  Task<IEnumerable<Professor>> ObterProfessoresPorEspecialidadeAsync(string especialidade, CancellationToken cancellationToken = default);
  Task<IEnumerable<Professor>> ObterProfessoresQuePodeLecionarAsync(TipoCurso tipoCurso, CancellationToken cancellationToken = default);
}
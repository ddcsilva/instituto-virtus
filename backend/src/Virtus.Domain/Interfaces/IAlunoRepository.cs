using Virtus.Domain.Entities;
using Virtus.Domain.Enums;

namespace Virtus.Domain.Interfaces;

/// <summary>
/// Interface específica para o repositório de alunos
/// </summary>
public interface IAlunoRepository : IRepository<Aluno>
{
  Task<Aluno?> ObterPorPessoaIdAsync(int pessoaId, CancellationToken cancellationToken = default);
  Task<IEnumerable<Aluno>> ObterPorStatusAsync(StatusAluno status, CancellationToken cancellationToken = default);
  Task<IEnumerable<Aluno>> ObterPorResponsavelIdAsync(int responsavelId, CancellationToken cancellationToken = default);
  Task<IEnumerable<Aluno>> ObterAlunosComMatriculasAtivasAsync(CancellationToken cancellationToken = default);
  Task<IEnumerable<Aluno>> ObterAlunosEmListaEsperaAsync(int turmaId, CancellationToken cancellationToken = default);
}
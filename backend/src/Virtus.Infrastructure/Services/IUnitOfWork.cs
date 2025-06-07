using Virtus.Domain.Interfaces;

namespace Virtus.Infrastructure.Services;

public interface IUnitOfWork : IDisposable
{
  IAlunoRepository Alunos { get; }
  IProfessorRepository Professores { get; }
  ITurmaRepository Turmas { get; }
  IMatriculaRepository Matriculas { get; }
  IPagamentoRepository Pagamentos { get; }

  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
  Task BeginTransactionAsync(CancellationToken cancellationToken = default);
  Task CommitTransactionAsync(CancellationToken cancellationToken = default);
  Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
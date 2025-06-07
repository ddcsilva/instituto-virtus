using Virtus.Domain.Interfaces;
using Virtus.Infrastructure.Data;
using Virtus.Infrastructure.Repositories;

namespace Virtus.Infrastructure.Services;

public class UnitOfWork : IUnitOfWork
{
  private readonly VirtusDbContext _context;
  private Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? _transaction;

  // Repositórios
  private IAlunoRepository? _alunoRepository;
  private IProfessorRepository? _professorRepository;
  private ITurmaRepository? _turmaRepository;
  private IMatriculaRepository? _matriculaRepository;
  private IPagamentoRepository? _pagamentoRepository;

  public UnitOfWork(VirtusDbContext context)
  {
    _context = context;
  }

  public IAlunoRepository Alunos => _alunoRepository ??= new AlunoRepository(_context);
  public IProfessorRepository Professores => _professorRepository ??= new ProfessorRepository(_context);
  public ITurmaRepository Turmas => _turmaRepository ??= new TurmaRepository(_context);
  public IMatriculaRepository Matriculas => _matriculaRepository ??= new MatriculaRepository(_context);
  public IPagamentoRepository Pagamentos => _pagamentoRepository ??= new PagamentoRepository(_context);

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    return await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
  {
    _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
  }

  public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
  {
    if (_transaction != null)
    {
      await _transaction.CommitAsync(cancellationToken);
      await _transaction.DisposeAsync();
      _transaction = null;
    }
  }

  public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
  {
    if (_transaction != null)
    {
      await _transaction.RollbackAsync(cancellationToken);
      await _transaction.DisposeAsync();
      _transaction = null;
    }
  }

  public void Dispose()
  {
    _transaction?.Dispose();
    _context.Dispose();
  }
}
using Microsoft.EntityFrameworkCore;
using Virtus.Domain.Entities;
using Virtus.Domain.Enums;
using Virtus.Domain.Interfaces;
using Virtus.Infrastructure.Data;

namespace Virtus.Infrastructure.Repositories;

public class PagamentoRepository : BaseRepository<Pagamento>, IPagamentoRepository
{
  public PagamentoRepository(VirtusDbContext context) : base(context) { }

  public override async Task<Pagamento?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(p => p.Pagador)
      .Include(p => p.PagamentoAlunos)
        .ThenInclude(pa => pa.Aluno)
          .ThenInclude(a => a.Pessoa)
      .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
  }

  public async Task<IEnumerable<Pagamento>> ObterPagamentosPorPagadorAsync(int pagadorId, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(p => p.PagamentoAlunos)
        .ThenInclude(pa => pa.Aluno)
          .ThenInclude(a => a.Pessoa)
      .Where(p => p.PagadorId == pagadorId)
      .OrderByDescending(p => p.DataPagamento)
      .ToListAsync(cancellationToken);
  }

  public async Task<IEnumerable<Pagamento>> ObterPagamentosPorAlunoAsync(int alunoId, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(p => p.Pagador)
      .Include(p => p.PagamentoAlunos)
      .Where(p => p.PagamentoAlunos.Any(pa => pa.AlunoId == alunoId))
      .OrderByDescending(p => p.DataPagamento)
      .ToListAsync(cancellationToken);
  }

  public async Task<IEnumerable<Pagamento>> ObterPagamentosPorStatusAsync(StatusPagamento status, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(p => p.Pagador)
      .Include(p => p.PagamentoAlunos)
        .ThenInclude(pa => pa.Aluno)
          .ThenInclude(a => a.Pessoa)
      .Where(p => p.Status == status)
      .OrderByDescending(p => p.DataVencimento)
      .ToListAsync(cancellationToken);
  }

  public async Task<IEnumerable<Pagamento>> ObterPagamentosPorPeriodoAsync(DateTime dataInicio, DateTime dataFim, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Include(p => p.Pagador)
      .Where(p => p.DataPagamento >= dataInicio && p.DataPagamento <= dataFim)
      .OrderByDescending(p => p.DataPagamento)
      .ToListAsync(cancellationToken);
  }

  public async Task<decimal> ObterTotalRecebidoPorPeriodoAsync(DateTime dataInicio, DateTime dataFim, CancellationToken cancellationToken = default)
  {
    return await _dbSet
      .Where(p => p.Status == StatusPagamento.Pago && p.DataPagamento >= dataInicio && p.DataPagamento <= dataFim)
      .SumAsync(p => p.Valor, cancellationToken);
  }

  public async Task<IEnumerable<Pagamento>> ObterPagamentosVencidosAsync(CancellationToken cancellationToken = default)
  {
    var dataAtual = DateTime.UtcNow.Date;

    return await _dbSet
      .Include(p => p.Pagador)
      .Include(p => p.PagamentoAlunos)
        .ThenInclude(pa => pa.Aluno)
          .ThenInclude(a => a.Pessoa)
      .Where(p => p.Status == StatusPagamento.Pendente && p.DataVencimento < dataAtual)
      .OrderBy(p => p.DataVencimento)
      .ToListAsync(cancellationToken);
  }
}
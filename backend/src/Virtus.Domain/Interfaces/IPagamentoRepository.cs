using Virtus.Domain.Entities;
using Virtus.Domain.Enums;

namespace Virtus.Domain.Interfaces;

/// <summary>
/// Interface específica para o repositório de pagamentos
/// </summary>
public interface IPagamentoRepository : IRepository<Pagamento>
{
  Task<IEnumerable<Pagamento>> ObterPagamentosPorPagadorAsync(int pagadorId, CancellationToken cancellationToken = default);
  Task<IEnumerable<Pagamento>> ObterPagamentosPorAlunoAsync(int alunoId, CancellationToken cancellationToken = default);
  Task<IEnumerable<Pagamento>> ObterPagamentosPorStatusAsync(StatusPagamento status, CancellationToken cancellationToken = default);
  Task<IEnumerable<Pagamento>> ObterPagamentosPorPeriodoAsync(DateTime dataInicio, DateTime dataFim, CancellationToken cancellationToken = default);
  Task<decimal> ObterTotalRecebidoPorPeriodoAsync(DateTime dataInicio, DateTime dataFim, CancellationToken cancellationToken = default);
  Task<IEnumerable<Pagamento>> ObterPagamentosVencidosAsync(CancellationToken cancellationToken = default);
}
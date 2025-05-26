using Virtus.Domain.Entities;

namespace Virtus.Domain.Interfaces;

public interface IPagamentoRepository : IBaseRepository<Pagamento>
{
    public Task<IEnumerable<Pagamento>> ObterPorPeriodoAsync(DateTime dataInicial, DateTime dataFinal);
    public Task<IEnumerable<Pagamento>> ObterPorPagadorAsync(int pagadorId);
    public Task<IEnumerable<Pagamento>> ObterPorAlunoAsync(int alunoId);
    public Task<Pagamento?> ObterComAlunosAsync(int id);
}

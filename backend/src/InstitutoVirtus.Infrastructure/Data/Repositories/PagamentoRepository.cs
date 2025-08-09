using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using InstitutoVirtus.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoVirtus.Infrastructure.Data.Repositories;

public class PagamentoRepository : BaseRepository<Pagamento>, IPagamentoRepository
{
    public PagamentoRepository(VirtusDbContext context) : base(context) { }

    public async Task<IEnumerable<Pagamento>> GetByResponsavelAsync(Guid responsavelId, CancellationToken cancellationToken = default)
    {
        return await _context.Pagamentos
            .Include(p => p.Parcelas)
                .ThenInclude(pp => pp.Mensalidade)
                    .ThenInclude(m => m.Matricula)
                        .ThenInclude(mat => mat.Aluno)
            .Where(p => p.ResponsavelId == responsavelId)
            .OrderByDescending(p => p.DataPagamento)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Pagamento>> GetByPeriodoAsync(DateTime inicio, DateTime fim, CancellationToken cancellationToken = default)
    {
        return await _context.Pagamentos
            .Include(p => p.Responsavel)
            .Include(p => p.Parcelas)
            .Where(p => p.DataPagamento >= inicio && p.DataPagamento <= fim)
            .OrderByDescending(p => p.DataPagamento)
            .ToListAsync(cancellationToken);
    }

    public async Task<Pagamento?> GetByReferenciaExternaAsync(string referencia, CancellationToken cancellationToken = default)
    {
        return await _context.Pagamentos
            .Include(p => p.Parcelas)
            .FirstOrDefaultAsync(p => p.ReferenciaExterna == referencia, cancellationToken);
    }

    public override async Task<Pagamento?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Pagamentos
            .Include(p => p.Responsavel)
            .Include(p => p.Parcelas)
                .ThenInclude(pp => pp.Mensalidade)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}

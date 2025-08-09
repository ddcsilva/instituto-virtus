using InstitutoVirtus.Domain.Interfaces.Repositories;
using InstitutoVirtus.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoVirtus.Infrastructure.Data.Repositories;

public class AulaRepository : BaseRepository<Aula>, IAulaRepository
{
    public AulaRepository(VirtusDbContext context) : base(context) { }

    public async Task<IEnumerable<Aula>> GetByTurmaAsync(Guid turmaId, CancellationToken cancellationToken = default)
    {
        return await _context.Aulas
            .Include(a => a.Presencas)
                .ThenInclude(p => p.Aluno)
            .Where(a => a.TurmaId == turmaId)
            .OrderBy(a => a.DataAula)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Aula>> GetByPeriodoAsync(DateTime inicio, DateTime fim, CancellationToken cancellationToken = default)
    {
        return await _context.Aulas
            .Include(a => a.Turma)
                .ThenInclude(t => t.Curso)
            .Include(a => a.Turma)
                .ThenInclude(t => t.Professor)
            .Where(a => a.DataAula >= inicio && a.DataAula <= fim)
            .OrderBy(a => a.DataAula)
            .ToListAsync(cancellationToken);
    }

    public async Task<Aula?> GetByTurmaAndDataAsync(Guid turmaId, DateTime data, CancellationToken cancellationToken = default)
    {
        return await _context.Aulas
            .Include(a => a.Presencas)
            .FirstOrDefaultAsync(a => a.TurmaId == turmaId && a.DataAula.Date == data.Date, cancellationToken);
    }

    public override async Task<Aula?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Aulas
            .Include(a => a.Turma)
            .Include(a => a.Presencas)
                .ThenInclude(p => p.Aluno)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }
}

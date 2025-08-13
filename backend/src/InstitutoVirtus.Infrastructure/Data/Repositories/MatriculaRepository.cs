using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using InstitutoVirtus.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoVirtus.Infrastructure.Data.Repositories;

public class MatriculaRepository : BaseRepository<Matricula>, IMatriculaRepository
{
    public MatriculaRepository(VirtusDbContext context) : base(context) { }

    public async Task<IEnumerable<Matricula>> GetByAlunoAsync(Guid alunoId, CancellationToken cancellationToken = default)
    {
        return await _context.Matriculas
            .Include(m => m.Turma)
                .ThenInclude(t => t.Curso)
            .Include(m => m.Turma)
                .ThenInclude(t => t.Professor)
            .Include(m => m.Mensalidades)
            .Where(m => m.AlunoId == alunoId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Matricula>> GetByTurmaAsync(Guid turmaId, CancellationToken cancellationToken = default)
    {
        return await _context.Matriculas
            .Include(m => m.Aluno)
            .Include(m => m.Mensalidades)
            .Where(m => m.TurmaId == turmaId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Matricula>> GetAtivasAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Matriculas
            .Include(m => m.Aluno)
            .Include(m => m.Turma)
                .ThenInclude(t => t.Curso)
            .Where(m => m.Status == StatusMatricula.Ativa)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AlunoJaMatriculadoNaTurmaAsync(Guid alunoId, Guid turmaId, CancellationToken cancellationToken = default)
    {
        return await _context.Matriculas
            .AnyAsync(m =>
                m.AlunoId == alunoId &&
                m.TurmaId == turmaId &&
                (m.Status == StatusMatricula.Ativa || m.Status == StatusMatricula.Trancada),
                cancellationToken);
    }

    public override async Task<Matricula?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Matriculas
            .Include(m => m.Aluno)
            .Include(m => m.Turma)
                .ThenInclude(t => t.Curso)
            .Include(m => m.Mensalidades)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<Matricula>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Matriculas
            .Include(m => m.Aluno)
            .Include(m => m.Turma)
                .ThenInclude(t => t.Curso)
            .ToListAsync(cancellationToken);
    }
}

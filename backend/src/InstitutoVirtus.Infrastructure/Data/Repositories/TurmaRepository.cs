using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using InstitutoVirtus.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoVirtus.Infrastructure.Data.Repositories;

public class TurmaRepository : BaseRepository<Turma>, ITurmaRepository
{
    public TurmaRepository(VirtusDbContext context) : base(context) { }

    public async Task<IEnumerable<Turma>> GetByCursoAsync(Guid cursoId, CancellationToken cancellationToken = default)
    {
        return await _context.Turmas
            .Include(t => t.Curso)
            .Include(t => t.Professor)
            .Include(t => t.Matriculas)
            .Where(t => t.CursoId == cursoId && t.Ativo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Turma>> GetByProfessorAsync(Guid professorId, CancellationToken cancellationToken = default)
    {
        return await _context.Turmas
            .Include(t => t.Curso)
            .Include(t => t.Matriculas)
            .Where(t => t.ProfessorId == professorId && t.Ativo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Turma>> GetByPeriodoAsync(int ano, int periodo, CancellationToken cancellationToken = default)
    {
        return await _context.Turmas
            .Include(t => t.Curso)
            .Include(t => t.Professor)
            .Include(t => t.Matriculas)
            .Where(t => t.AnoLetivo == ano && t.Periodo == periodo && t.Ativo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Turma>> GetByAnoLetivoAsync(int ano, CancellationToken cancellationToken = default)
    {
        return await _context.Turmas
            .Include(t => t.Curso)
            .Include(t => t.Professor)
            .Include(t => t.Matriculas)
            .Where(t => t.AnoLetivo == ano && t.Ativo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Turma>> GetComVagasAsync(CancellationToken cancellationToken = default)
    {
        var turmas = await _context.Turmas
            .Include(t => t.Curso)
            .Include(t => t.Professor)
            .Include(t => t.Matriculas.Where(m => m.Status == StatusMatricula.Ativa))
            .Where(t => t.Ativo)
            .ToListAsync(cancellationToken);

        return turmas.Where(t => t.TemVaga());
    }

    public async Task<bool> ExisteConflitoHorarioAsync(
        Guid professorId,
        DiaSemana dia,
        TimeSpan horaInicio,
        CancellationToken cancellationToken = default)
    {
        return await _context.Turmas
            .AnyAsync(t =>
                t.ProfessorId == professorId &&
                t.DiaSemana == dia &&
                t.Horario.HoraInicio == horaInicio &&
                t.Ativo,
                cancellationToken);
    }

    public override async Task<Turma?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Turmas
            .Include(t => t.Curso)
            .Include(t => t.Professor)
            .Include(t => t.Matriculas)
                .ThenInclude(m => m.Aluno)
            .Include(t => t.Aulas)
            .Include(t => t.Avaliacoes)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }
}

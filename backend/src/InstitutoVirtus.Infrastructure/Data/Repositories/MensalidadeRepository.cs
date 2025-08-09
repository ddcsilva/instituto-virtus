using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using InstitutoVirtus.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoVirtus.Infrastructure.Data.Repositories;

public class MensalidadeRepository : BaseRepository<Mensalidade>, IMensalidadeRepository
{
    public MensalidadeRepository(VirtusDbContext context) : base(context) { }

    public async Task<IEnumerable<Mensalidade>> GetByMatriculaAsync(Guid matriculaId, CancellationToken cancellationToken = default)
    {
        return await _context.Mensalidades
            .Include(m => m.Matricula)
                .ThenInclude(mat => mat.Aluno)
            .Where(m => m.MatriculaId == matriculaId)
            .OrderBy(m => m.Competencia.Ano)
            .ThenBy(m => m.Competencia.Mes)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Mensalidade>> GetByCompetenciaAsync(int ano, int mes, CancellationToken cancellationToken = default)
    {
        return await _context.Mensalidades
            .Include(m => m.Matricula)
                .ThenInclude(mat => mat.Aluno)
            .Include(m => m.Matricula)
                .ThenInclude(mat => mat.Turma)
                    .ThenInclude(t => t.Curso)
            .Where(m => m.Competencia.Ano == ano && m.Competencia.Mes == mes)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Mensalidade>> GetVencidasAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Mensalidades
            .Include(m => m.Matricula)
                .ThenInclude(mat => mat.Aluno)
                    .ThenInclude(a => a.Responsaveis)
                        .ThenInclude(ra => ra.Responsavel)
            .Where(m => m.Status == StatusMensalidade.Vencido)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Mensalidade>> GetEmAbertoByResponsavelAsync(Guid responsavelId, CancellationToken cancellationToken = default)
    {
        var alunosIds = await _context.ResponsaveisAlunos
            .Where(ra => ra.ResponsavelId == responsavelId)
            .Select(ra => ra.AlunoId)
            .ToListAsync(cancellationToken);

        return await _context.Mensalidades
            .Include(m => m.Matricula)
                .ThenInclude(mat => mat.Aluno)
            .Include(m => m.Matricula)
                .ThenInclude(mat => mat.Turma)
                    .ThenInclude(t => t.Curso)
            .Where(m =>
                alunosIds.Contains(m.Matricula.AlunoId) &&
                (m.Status == StatusMensalidade.EmAberto || m.Status == StatusMensalidade.Vencido))
            .OrderBy(m => m.DataVencimento)
            .ToListAsync(cancellationToken);
    }
}

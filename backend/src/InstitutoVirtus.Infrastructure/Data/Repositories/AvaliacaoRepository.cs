using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using InstitutoVirtus.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoVirtus.Infrastructure.Data.Repositories;

public class AvaliacaoRepository : BaseRepository<Avaliacao>, IAvaliacaoRepository
{
    public AvaliacaoRepository(VirtusDbContext context) : base(context) { }

    public async Task<IEnumerable<Avaliacao>> GetByTurmaAsync(Guid turmaId, CancellationToken cancellationToken = default)
    {
        return await _context.Avaliacoes
            .Include(a => a.Notas)
                .ThenInclude(n => n.Aluno)
            .Where(a => a.TurmaId == turmaId)
            .OrderBy(a => a.DataAplicacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> CalcularMediaFinalAsync(Guid alunoId, Guid turmaId, CancellationToken cancellationToken = default)
    {
        var avaliacoes = await _context.Avaliacoes
            .Include(a => a.Notas.Where(n => n.AlunoId == alunoId))
            .Where(a => a.TurmaId == turmaId)
            .ToListAsync(cancellationToken);

        if (!avaliacoes.Any() || !avaliacoes.SelectMany(a => a.Notas).Any())
            return 0;

        decimal somaNotasPonderadas = 0;
        decimal somaPesos = 0;

        foreach (var avaliacao in avaliacoes)
        {
            var nota = avaliacao.Notas.FirstOrDefault();
            if (nota != null)
            {
                somaNotasPonderadas += nota.Valor * avaliacao.Peso;
                somaPesos += avaliacao.Peso;
            }
        }

        return somaPesos > 0 ? Math.Round(somaNotasPonderadas / somaPesos, 2) : 0;
    }

    public async Task<double> CalcularFrequenciaAsync(Guid alunoId, Guid turmaId, CancellationToken cancellationToken = default)
    {
        var aulas = await _context.Aulas
            .Include(a => a.Presencas.Where(p => p.AlunoId == alunoId))
            .Where(a => a.TurmaId == turmaId && a.Realizada)
            .ToListAsync(cancellationToken);

        if (!aulas.Any())
            return 100;

        var totalAulas = aulas.Count;
        var totalPresencas = aulas
            .SelectMany(a => a.Presencas)
            .Count(p => p.Status == StatusPresenca.Presente || p.Status == StatusPresenca.Justificada);

        return Math.Round((double)totalPresencas / totalAulas * 100, 2);
    }

    public override async Task<Avaliacao?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Avaliacoes
            .Include(a => a.Turma)
            .Include(a => a.Notas)
                .ThenInclude(n => n.Aluno)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }
}

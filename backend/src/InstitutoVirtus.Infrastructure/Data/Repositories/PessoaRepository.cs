using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Enums;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using InstitutoVirtus.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace InstitutoVirtus.Infrastructure.Data.Repositories;

public class PessoaRepository : BaseRepository<Pessoa>, IPessoaRepository
{
    public PessoaRepository(VirtusDbContext context) : base(context) { }

    public async Task<Pessoa?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Pessoas
            .FirstOrDefaultAsync(p => p.Email != null && p.Email.Endereco == email.ToLower(), cancellationToken);
    }

    public async Task<IEnumerable<Pessoa>> GetByTipoAsync(TipoPessoa tipo, CancellationToken cancellationToken = default)
    {
        return await _context.Pessoas
            .Where(p => p.TipoPessoa == tipo && p.Ativo)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByTelefoneAsync(string telefone, CancellationToken cancellationToken = default)
    {
        var telefoneLimpo = new string(telefone.Where(char.IsDigit).ToArray());
        return await _context.Pessoas
            .AnyAsync(p => p.Telefone.Numero == telefoneLimpo, cancellationToken);
    }

    public async Task<IEnumerable<Aluno>> GetAlunosByResponsavelAsync(Guid responsavelId, CancellationToken cancellationToken = default)
    {
        return await _context.Alunos
            .Include(a => a.Responsaveis)
            .Include(a => a.Matriculas)
                .ThenInclude(m => m.Turma)
                    .ThenInclude(t => t.Curso)
            .Where(a => a.Responsaveis.Any(r => r.ResponsavelId == responsavelId))
            .ToListAsync(cancellationToken);
    }

    public override async Task<Pessoa?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Pessoas
            .Include(p => (p as Aluno)!.Responsaveis)
                .ThenInclude(ra => ra.Responsavel)
            .Include(p => (p as Aluno)!.Matriculas)
            .Include(p => (p as Professor)!.Turmas)
            .Include(p => (p as Responsavel)!.Alunos)
                .ThenInclude(ra => ra.Aluno)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<(IEnumerable<Pessoa> Items, int TotalCount)> SearchAsync(
        string? nome,
        TipoPessoa? tipo,
        bool? ativo,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Pessoas.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
        {
            var search = nome.Trim().ToLower();
            query = query.Where(p =>
                p.NomeCompleto.ToLower().Contains(search) ||
                p.Telefone.Numero.Contains(search) ||
                (p.Email != null && p.Email.Endereco.ToLower().Contains(search))
            );
        }

        if (tipo.HasValue)
        {
            query = query.Where(p => p.TipoPessoa == tipo.Value);
        }

        if (ativo.HasValue)
        {
            query = query.Where(p => p.Ativo == ativo.Value);
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.NomeCompleto)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}

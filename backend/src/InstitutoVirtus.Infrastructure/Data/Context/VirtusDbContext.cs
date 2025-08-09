namespace InstitutoVirtus.Infrastructure.Data.Context;

using Microsoft.EntityFrameworkCore;
using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Common;
using InstitutoVirtus.Application.Common.Interfaces;
using System.Reflection;

public class VirtusDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;

    public VirtusDbContext(
        DbContextOptions<VirtusDbContext> options,
        ICurrentUserService currentUserService,
        IDateTime dateTime) : base(options)
    {
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }

    public DbSet<Pessoa> Pessoas { get; set; }
    public DbSet<Aluno> Alunos { get; set; }
    public DbSet<Professor> Professores { get; set; }
    public DbSet<Responsavel> Responsaveis { get; set; }
    public DbSet<ResponsavelAluno> ResponsaveisAlunos { get; set; }
    public DbSet<Curso> Cursos { get; set; }
    public DbSet<Turma> Turmas { get; set; }
    public DbSet<Matricula> Matriculas { get; set; }
    public DbSet<Mensalidade> Mensalidades { get; set; }
    public DbSet<Pagamento> Pagamentos { get; set; }
    public DbSet<PagamentoParcela> PagamentosParcelas { get; set; }
    public DbSet<Aula> Aulas { get; set; }
    public DbSet<Presenca> Presencas { get; set; }
    public DbSet<Avaliacao> Avaliacoes { get; set; }
    public DbSet<Nota> Notas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCriadoPor(_currentUserService.UserName ?? "Sistema");
                    entry.Entity.DataCriacao = _dateTime.Now;
                    break;

                case EntityState.Modified:
                    entry.Entity.SetAtualizadoPor(_currentUserService.UserName ?? "Sistema");
                    entry.Entity.DataAtualizacao = _dateTime.Now;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
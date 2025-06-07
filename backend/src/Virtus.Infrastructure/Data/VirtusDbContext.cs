using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Virtus.Domain.Entities;

namespace Virtus.Infrastructure.Data;

/// <summary>
/// Contexto do banco de dados para a aplicação Virtus.
/// </summary>
public class VirtusDbContext : DbContext
{
  public VirtusDbContext(DbContextOptions<VirtusDbContext> options) : base(options) { }

  public DbSet<Pessoa> Pessoas => Set<Pessoa>();
  public DbSet<Aluno> Alunos => Set<Aluno>();
  public DbSet<Professor> Professores => Set<Professor>();
  public DbSet<Turma> Turmas => Set<Turma>();
  public DbSet<Matricula> Matriculas => Set<Matricula>();
  public DbSet<Pagamento> Pagamentos => Set<Pagamento>();
  public DbSet<PagamentoAluno> PagamentosAlunos => Set<PagamentoAluno>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

    // Configuração global para DateTime em UTC
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
      var dateTimeProperties = entityType.GetProperties()
        .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?));

      foreach (var property in dateTimeProperties)
      {
        property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
          v => v.ToUniversalTime(),
          v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
      }
    }
  }

  public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    // Atualiza automaticamente as datas de criação e atualização
    foreach (var entry in ChangeTracker.Entries<BaseEntity>())
    {
      switch (entry.State)
      {
        case EntityState.Added:
          entry.Entity.GetType().GetProperty("CriadoEm")?.SetValue(entry.Entity, DateTime.UtcNow);
          break;
        case EntityState.Modified:
          entry.Entity.GetType().GetProperty("AtualizadoEm")?.SetValue(entry.Entity, DateTime.UtcNow);
          break;
      }
    }

    return base.SaveChangesAsync(cancellationToken);
  }
}
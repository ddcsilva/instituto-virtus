using Microsoft.EntityFrameworkCore;

namespace Virtus.Infrastructure.Data;

/// <summary>
/// Contexto do banco de dados para a aplicação Virtus.
/// </summary>
public class VirtusDbContext : DbContext
{
  public VirtusDbContext(DbContextOptions<VirtusDbContext> options) : base(options) { }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Configurações futuras
  }
}
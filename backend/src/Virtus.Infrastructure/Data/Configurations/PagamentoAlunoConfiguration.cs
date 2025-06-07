
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Virtus.Domain.Entities;

namespace Virtus.Infrastructure.Data.Configurations;

public class PagamentoAlunoConfiguration : IEntityTypeConfiguration<PagamentoAluno>
{
  public void Configure(EntityTypeBuilder<PagamentoAluno> builder)
  {
    builder.ToTable("PagamentosAlunos");

    builder.HasKey(pa => pa.Id);

    builder.Property(pa => pa.Id)
      .ValueGeneratedOnAdd();

    builder.Property(pa => pa.ValorProporcional)
      .IsRequired()
      .HasPrecision(10, 2);

    builder.Property(pa => pa.Observacao)
      .HasMaxLength(500);

    // Relacionamento com Pagamento (N:1)
    builder.HasOne(pa => pa.Pagamento)
      .WithMany(p => p.PagamentoAlunos)
      .HasForeignKey(pa => pa.PagamentoId)
      .OnDelete(DeleteBehavior.Cascade);

    // Relacionamento com Aluno (N:1)
    builder.HasOne(pa => pa.Aluno)
      .WithMany()
      .HasForeignKey(pa => pa.AlunoId)
      .OnDelete(DeleteBehavior.Restrict);

    // Índices
    builder.HasIndex(pa => new { pa.PagamentoId, pa.AlunoId })
      .IsUnique();

    builder.HasIndex(pa => pa.AlunoId);
  }
}
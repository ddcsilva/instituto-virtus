using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Virtus.Domain.Entities;

namespace Virtus.Infrastructure.Data.Configurations;

public class PagamentoConfiguration : IEntityTypeConfiguration<Pagamento>
{
  public void Configure(EntityTypeBuilder<Pagamento> builder)
  {
    builder.ToTable("Pagamentos");

    builder.HasKey(p => p.Id);

    builder.Property(p => p.Id)
      .ValueGeneratedOnAdd();

    builder.Property(p => p.Valor)
      .IsRequired()
      .HasPrecision(10, 2);

    builder.Property(p => p.DataPagamento)
      .IsRequired();

    builder.Property(p => p.DataVencimento)
      .IsRequired();

    builder.Property(p => p.Status)
      .IsRequired()
      .HasConversion<int>();

    builder.Property(p => p.Observacao)
      .HasMaxLength(500);

    builder.Property(p => p.NumeroTransacao)
      .HasMaxLength(100);

    builder.Property(p => p.FormaPagamento)
      .IsRequired()
      .HasMaxLength(50);

    // Relacionamento com Pagador (N:1)
    builder.HasOne(p => p.Pagador)
      .WithMany()
      .HasForeignKey(p => p.PagadorId)
      .OnDelete(DeleteBehavior.Restrict);

    // Relacionamento com PagamentoAluno (1:N)
    builder.HasMany(p => p.PagamentoAlunos)
      .WithOne(pa => pa.Pagamento)
      .HasForeignKey(pa => pa.PagamentoId)
      .OnDelete(DeleteBehavior.Cascade);

    // Índices
    builder.HasIndex(p => p.PagadorId);
    builder.HasIndex(p => p.Status);
    builder.HasIndex(p => p.DataVencimento);
    builder.HasIndex(p => new { p.Status, p.DataVencimento });
  }
}
using InstitutoVirtus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoVirtus.Infrastructure.Data.Configurations;

public class PagamentoConfiguration : IEntityTypeConfiguration<Pagamento>
{
    public void Configure(EntityTypeBuilder<Pagamento> builder)
    {
        builder.ToTable("Pagamentos");

        builder.HasKey(p => p.Id);

        builder.OwnsOne(p => p.ValorTotal, valor =>
        {
            valor.Property(v => v.Valor)
                .HasColumnName("ValorTotal")
                .HasPrecision(10, 2)
                .IsRequired();
        });

        builder.Property(p => p.DataPagamento)
            .IsRequired();

        builder.Property(p => p.MeioPagamento)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.ReferenciaExterna)
            .HasMaxLength(100);

        builder.Property(p => p.ComprovanteUrl)
            .HasMaxLength(500);

        builder.Property(p => p.Observacoes)
            .HasMaxLength(500);

        builder.HasOne(p => p.Responsavel)
            .WithMany(r => r.Pagamentos)
            .HasForeignKey(p => p.ResponsavelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Parcelas)
            .WithOne(pp => pp.Pagamento)
            .HasForeignKey(pp => pp.PagamentoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(p => p.DataPagamento);
        builder.HasIndex(p => p.ResponsavelId);
        builder.HasIndex(p => p.ReferenciaExterna).IsUnique().HasFilter("ReferenciaExterna IS NOT NULL");
    }
}

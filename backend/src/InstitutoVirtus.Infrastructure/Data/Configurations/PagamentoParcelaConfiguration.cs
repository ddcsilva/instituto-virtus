using InstitutoVirtus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoVirtus.Infrastructure.Data.Configurations;

public class PagamentoParcelaConfiguration : IEntityTypeConfiguration<PagamentoParcela>
{
    public void Configure(EntityTypeBuilder<PagamentoParcela> builder)
    {
        builder.ToTable("PagamentosParcelas");

        builder.HasKey(pp => pp.Id);

        builder.OwnsOne(pp => pp.ValorAlocado, valor =>
        {
            valor.Property(v => v.Valor)
                .HasColumnName("ValorAlocado")
                .HasPrecision(10, 2)
                .IsRequired();
        });

        builder.HasOne(pp => pp.Pagamento)
            .WithMany(p => p.Parcelas)
            .HasForeignKey(pp => pp.PagamentoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pp => pp.Mensalidade)
            .WithMany()
            .HasForeignKey(pp => pp.MensalidadeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(pp => new { pp.PagamentoId, pp.MensalidadeId }).IsUnique();
    }
}

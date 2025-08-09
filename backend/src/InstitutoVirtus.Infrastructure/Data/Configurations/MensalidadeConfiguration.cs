using InstitutoVirtus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoVirtus.Infrastructure.Data.Configurations;

public class MensalidadeConfiguration : IEntityTypeConfiguration<Mensalidade>
{
    public void Configure(EntityTypeBuilder<Mensalidade> builder)
    {
        builder.ToTable("Mensalidades");

        builder.HasKey(m => m.Id);

        builder.OwnsOne(m => m.Competencia, competencia =>
        {
            competencia.Property(c => c.Ano)
                .HasColumnName("CompetenciaAno")
                .IsRequired();

            competencia.Property(c => c.Mes)
                .HasColumnName("CompetenciaMes")
                .IsRequired();

            competencia.HasIndex(c => new { c.Ano, c.Mes });
        });

        builder.OwnsOne(m => m.Valor, valor =>
        {
            valor.Property(v => v.Valor)
                .HasColumnName("Valor")
                .HasPrecision(10, 2)
                .IsRequired();
        });

        builder.Property(m => m.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(m => m.DataVencimento)
            .IsRequired();

        builder.Property(m => m.FormaPagamento)
            .HasConversion<string>();

        builder.Property(m => m.Observacao)
            .HasMaxLength(500);

        builder.HasOne(m => m.Matricula)
            .WithMany(mat => mat.Mensalidades)
            .HasForeignKey(m => m.MatriculaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(m => m.Status);
        builder.HasIndex(m => m.DataVencimento);
        builder.HasIndex(m => m.MatriculaId);
    }
}

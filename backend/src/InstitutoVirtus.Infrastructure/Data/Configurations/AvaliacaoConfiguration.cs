using InstitutoVirtus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoVirtus.Infrastructure.Data.Configurations;

public class AvaliacaoConfiguration : IEntityTypeConfiguration<Avaliacao>
{
    public void Configure(EntityTypeBuilder<Avaliacao> builder)
    {
        builder.ToTable("Avaliacoes");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Peso)
            .IsRequired()
            .HasPrecision(3, 2)
            .HasDefaultValue(1);

        builder.Property(a => a.Descricao)
            .HasMaxLength(500);

        builder.HasOne(a => a.Turma)
            .WithMany(t => t.Avaliacoes)
            .HasForeignKey(a => a.TurmaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Notas)
            .WithOne(n => n.Avaliacao)
            .HasForeignKey(n => n.AvaliacaoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(a => a.TurmaId);
    }
}

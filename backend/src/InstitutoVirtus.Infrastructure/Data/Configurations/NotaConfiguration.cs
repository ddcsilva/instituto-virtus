using InstitutoVirtus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoVirtus.Infrastructure.Data.Configurations;

public class NotaConfiguration : IEntityTypeConfiguration<Nota>
{
    public void Configure(EntityTypeBuilder<Nota> builder)
    {
        builder.ToTable("Notas");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Valor)
            .IsRequired()
            .HasPrecision(4, 2);

        builder.Property(n => n.Observacoes)
            .HasMaxLength(500);

        builder.HasOne(n => n.Avaliacao)
            .WithMany(a => a.Notas)
            .HasForeignKey(n => n.AvaliacaoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.Aluno)
            .WithMany(a => a.Notas)
            .HasForeignKey(n => n.AlunoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(n => new { n.AvaliacaoId, n.AlunoId }).IsUnique();
    }
}

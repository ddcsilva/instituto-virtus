using InstitutoVirtus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoVirtus.Infrastructure.Data.Configurations;

public class PresencaConfiguration : IEntityTypeConfiguration<Presenca>
{
    public void Configure(EntityTypeBuilder<Presenca> builder)
    {
        builder.ToTable("Presencas");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.Justificativa)
            .HasMaxLength(500);

        builder.HasOne(p => p.Aula)
            .WithMany(a => a.Presencas)
            .HasForeignKey(p => p.AulaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Aluno)
            .WithMany(a => a.Presencas)
            .HasForeignKey(p => p.AlunoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(p => new { p.AulaId, p.AlunoId }).IsUnique();
        builder.HasIndex(p => p.Status);
    }
}

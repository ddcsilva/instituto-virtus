using InstitutoVirtus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoVirtus.Infrastructure.Data.Configurations;

public class AulaConfiguration : IEntityTypeConfiguration<Aula>
{
    public void Configure(EntityTypeBuilder<Aula> builder)
    {
        builder.ToTable("Aulas");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.DataAula)
            .IsRequired();

        builder.Property(a => a.Conteudo)
            .HasMaxLength(500);

        builder.Property(a => a.Observacoes)
            .HasMaxLength(500);

        builder.Property(a => a.Realizada)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne(a => a.Turma)
            .WithMany(t => t.Aulas)
            .HasForeignKey(a => a.TurmaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Presencas)
            .WithOne(p => p.Aula)
            .HasForeignKey(p => p.AulaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(a => new { a.TurmaId, a.DataAula }).IsUnique();
        builder.HasIndex(a => a.Realizada);
    }
}

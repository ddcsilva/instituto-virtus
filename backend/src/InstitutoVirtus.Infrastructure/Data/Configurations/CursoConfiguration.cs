using InstitutoVirtus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoVirtus.Infrastructure.Data.Configurations;

public class CursoConfiguration : IEntityTypeConfiguration<Curso>
{
    public void Configure(EntityTypeBuilder<Curso> builder)
    {
        builder.ToTable("Cursos");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Descricao)
            .HasMaxLength(500);

        builder.Property(c => c.ValorMensalidade)
            .IsRequired()
            .HasPrecision(10, 2);

        builder.Property(c => c.CargaHoraria)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(c => c.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasMany(c => c.Turmas)
            .WithOne(t => t.Curso)
            .HasForeignKey(t => t.CursoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(c => c.Nome).IsUnique();
        builder.HasIndex(c => c.Ativo);
    }
}

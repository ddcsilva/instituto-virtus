using InstitutoVirtus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoVirtus.Infrastructure.Data.Configurations;

public class MatriculaConfiguration : IEntityTypeConfiguration<Matricula>
{
    public void Configure(EntityTypeBuilder<Matricula> builder)
    {
        builder.ToTable("Matriculas");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.DataMatricula)
            .IsRequired();

        builder.Property(m => m.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(m => m.MotivoSaida)
            .HasMaxLength(500);

        builder.HasOne(m => m.Aluno)
            .WithMany(a => a.Matriculas)
            .HasForeignKey(m => m.AlunoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Turma)
            .WithMany(t => t.Matriculas)
            .HasForeignKey(m => m.TurmaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(m => m.Mensalidades)
            .WithOne(mens => mens.Matricula)
            .HasForeignKey(mens => mens.MatriculaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(m => new { m.AlunoId, m.TurmaId }).IsUnique();
        builder.HasIndex(m => m.Status);
        builder.HasIndex(m => m.DataMatricula);
    }
}

using InstitutoVirtus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoVirtus.Infrastructure.Data.Configurations;

public class TurmaConfiguration : IEntityTypeConfiguration<Turma>
{
    public void Configure(EntityTypeBuilder<Turma> builder)
    {
        builder.ToTable("Turmas");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.DiaSemana)
            .IsRequired()
            .HasConversion<string>();

        builder.OwnsOne(t => t.Horario, horario =>
        {
            horario.Property(h => h.HoraInicio)
                .HasColumnName("HoraInicio")
                .IsRequired();

            horario.Property(h => h.HoraFim)
                .HasColumnName("HoraFim")
                .IsRequired();
        });

        builder.Property(t => t.Capacidade)
            .IsRequired();

        builder.Property(t => t.Sala)
            .HasMaxLength(50);

        builder.Property(t => t.AnoLetivo)
            .IsRequired();

        builder.Property(t => t.Periodo)
            .IsRequired();

        builder.Property(t => t.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(t => t.Curso)
            .WithMany(c => c.Turmas)
            .HasForeignKey(t => t.CursoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Professor)
            .WithMany(p => p.Turmas)
            .HasForeignKey(t => t.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Matriculas)
            .WithOne(m => m.Turma)
            .HasForeignKey(m => m.TurmaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Aulas)
            .WithOne(a => a.Turma)
            .HasForeignKey(a => a.TurmaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Avaliacoes)
            .WithOne(av => av.Turma)
            .HasForeignKey(av => av.TurmaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(t => new { t.ProfessorId, t.DiaSemana, t.AnoLetivo, t.Periodo });
        builder.HasIndex(t => t.Ativo);
    }
}

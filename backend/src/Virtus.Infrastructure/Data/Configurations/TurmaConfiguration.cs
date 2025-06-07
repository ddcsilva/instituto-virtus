using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Virtus.Domain.Entities;

namespace Virtus.Infrastructure.Data.Configurations;

public class TurmaConfiguration : IEntityTypeConfiguration<Turma>
{
  public void Configure(EntityTypeBuilder<Turma> builder)
  {
    builder.ToTable("Turmas");

    builder.HasKey(t => t.Id);

    builder.Property(t => t.Id)
      .ValueGeneratedOnAdd();

    builder.Property(t => t.Nome)
      .IsRequired()
      .HasMaxLength(100);

    builder.Property(t => t.Capacidade)
      .IsRequired();

    builder.Property(t => t.Tipo)
      .IsRequired()
      .HasConversion<int>();

    builder.Property(t => t.Descricao)
      .HasMaxLength(500);

    builder.Property(t => t.Horario)
      .IsRequired()
      .HasMaxLength(20);

    builder.Property(t => t.DiaSemana)
      .IsRequired()
      .HasMaxLength(20);

    builder.Property(t => t.Ativa)
      .IsRequired()
      .HasDefaultValue(true);

    builder.Property(t => t.DataInicio)
      .IsRequired();

    builder.Property(t => t.DataTermino);

    // Relacionamento com Professor (N:1)
    builder.HasOne(t => t.Professor)
      .WithMany(p => p.Turmas)
      .HasForeignKey(t => t.ProfessorId)
      .OnDelete(DeleteBehavior.Restrict);

    // Relacionamento com Matriculas (1:N)
    builder.HasMany(t => t.Matriculas)
      .WithOne(m => m.Turma)
      .HasForeignKey(m => m.TurmaId)
      .OnDelete(DeleteBehavior.Restrict);

    // Índices
    builder.HasIndex(t => t.ProfessorId);
    builder.HasIndex(t => t.Tipo);
    builder.HasIndex(t => t.Ativa);
    builder.HasIndex(t => new { t.DiaSemana, t.Horario });
  }
}
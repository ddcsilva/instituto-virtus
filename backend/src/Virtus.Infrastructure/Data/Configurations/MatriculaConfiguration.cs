using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Virtus.Domain.Entities;

namespace Virtus.Infrastructure.Data.Configurations;

public class MatriculaConfiguration : IEntityTypeConfiguration<Matricula>
{
  public void Configure(EntityTypeBuilder<Matricula> builder)
  {
    builder.ToTable("Matriculas");

    builder.HasKey(m => m.Id);

    builder.Property(m => m.Id)
      .ValueGeneratedOnAdd();

    builder.Property(m => m.Status)
      .IsRequired()
      .HasConversion<int>();

    builder.Property(m => m.DataMatricula)
      .IsRequired();

    builder.Property(m => m.DataCancelamento);

    builder.Property(m => m.MotivoCancelamento)
      .HasMaxLength(500);

    builder.Property(m => m.NumeroOrdemEspera)
      .IsRequired()
      .HasDefaultValue(0);

    // Relacionamento com Aluno (N:1)
    builder.HasOne(m => m.Aluno)
      .WithMany(a => a.Matriculas)
      .HasForeignKey(m => m.AlunoId)
      .OnDelete(DeleteBehavior.Restrict);

    // Relacionamento com Turma (N:1)
    builder.HasOne(m => m.Turma)
      .WithMany(t => t.Matriculas)
      .HasForeignKey(m => m.TurmaId)
      .OnDelete(DeleteBehavior.Restrict);

    // Índices
    builder.HasIndex(m => new { m.AlunoId, m.TurmaId, m.Status })
      .HasFilter("[Status] = 1"); // Apenas matrículas ativas

    builder.HasIndex(m => m.TurmaId);
    builder.HasIndex(m => m.Status);
    builder.HasIndex(m => new { m.TurmaId, m.NumeroOrdemEspera })
      .HasFilter("[NumeroOrdemEspera] > 0");
  }
}
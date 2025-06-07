// backend/src/Virtus.Infrastructure/Data/Configurations/ProfessorConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Virtus.Domain.Entities;

namespace Virtus.Infrastructure.Data.Configurations;

public class ProfessorConfiguration : IEntityTypeConfiguration<Professor>
{
  public void Configure(EntityTypeBuilder<Professor> builder)
  {
    builder.ToTable("Professores");

    builder.HasKey(p => p.Id);

    builder.Property(p => p.Id)
      .ValueGeneratedOnAdd();

    builder.Property(p => p.Especialidade)
      .IsRequired()
      .HasMaxLength(100);

    builder.Property(p => p.DataAdmissao)
      .IsRequired();

    builder.Property(p => p.Ativo)
      .IsRequired()
      .HasDefaultValue(true);

    // Relacionamento com Pessoa (1:1)
    builder.HasOne(p => p.Pessoa)
      .WithOne()
      .HasForeignKey<Professor>(p => p.PessoaId)
      .OnDelete(DeleteBehavior.Restrict);

    // Relacionamento com Turmas (1:N)
    builder.HasMany(p => p.Turmas)
      .WithOne(t => t.Professor)
      .HasForeignKey(t => t.ProfessorId)
      .OnDelete(DeleteBehavior.Restrict);

    // Índices
    builder.HasIndex(p => p.PessoaId)
      .IsUnique();

    builder.HasIndex(p => p.Ativo);

    builder.HasIndex(p => p.Especialidade);
  }
}
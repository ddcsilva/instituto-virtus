using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Virtus.Domain.Entities;

namespace Virtus.Infrastructure.Data.Configurations;

public class AlunoConfiguration : IEntityTypeConfiguration<Aluno>
{
  public void Configure(EntityTypeBuilder<Aluno> builder)
  {
    builder.ToTable("Alunos");

    builder.HasKey(a => a.Id);

    builder.Property(a => a.Id)
      .ValueGeneratedOnAdd();

    builder.Property(a => a.Status)
      .IsRequired()
      .HasConversion<int>();

    builder.Property(a => a.DataMatricula)
      .IsRequired();

    builder.Property(a => a.Observacoes)
      .HasMaxLength(1000);

    // Relacionamento com Pessoa (1:1)
    builder.HasOne(a => a.Pessoa)
      .WithOne()
      .HasForeignKey<Aluno>(a => a.PessoaId)
      .OnDelete(DeleteBehavior.Restrict);

    // Relacionamento com Responsável (N:1)
    builder.HasOne(a => a.Responsavel)
      .WithMany()
      .HasForeignKey(a => a.ResponsavelId)
      .OnDelete(DeleteBehavior.SetNull);

    // Relacionamento com Matriculas (1:N)
    builder.HasMany(a => a.Matriculas)
      .WithOne(m => m.Aluno)
      .HasForeignKey(m => m.AlunoId)
      .OnDelete(DeleteBehavior.Restrict);

    // Índices
    builder.HasIndex(a => a.PessoaId)
      .IsUnique();

    builder.HasIndex(a => a.ResponsavelId);

    builder.HasIndex(a => a.Status);
  }
}
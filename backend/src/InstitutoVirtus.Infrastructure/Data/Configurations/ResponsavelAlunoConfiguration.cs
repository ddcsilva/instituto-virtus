using InstitutoVirtus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoVirtus.Infrastructure.Data.Configurations;

public class ResponsavelAlunoConfiguration : IEntityTypeConfiguration<ResponsavelAluno>
{
    public void Configure(EntityTypeBuilder<ResponsavelAluno> builder)
    {
        builder.ToTable("ResponsaveisAlunos");

        builder.HasKey(ra => ra.Id);

        builder.Property(ra => ra.Parentesco)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(ra => ra.Principal)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne(ra => ra.Responsavel)
            .WithMany(r => r.Alunos)
            .HasForeignKey(ra => ra.ResponsavelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ra => ra.Aluno)
            .WithMany(a => a.Responsaveis)
            .HasForeignKey(ra => ra.AlunoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(ra => new { ra.ResponsavelId, ra.AlunoId }).IsUnique();
        builder.HasIndex(ra => ra.Principal);
    }
}

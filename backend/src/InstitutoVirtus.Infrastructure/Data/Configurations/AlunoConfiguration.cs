using InstitutoVirtus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoVirtus.Infrastructure.Data.Configurations;

public class AlunoConfiguration : IEntityTypeConfiguration<Aluno>
{
    public void Configure(EntityTypeBuilder<Aluno> builder)
    {
        builder.HasMany(a => a.Matriculas)
            .WithOne(m => m.Aluno)
            .HasForeignKey(m => m.AlunoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Presencas)
            .WithOne(p => p.Aluno)
            .HasForeignKey(p => p.AlunoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Notas)
            .WithOne(n => n.Aluno)
            .HasForeignKey(n => n.AlunoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

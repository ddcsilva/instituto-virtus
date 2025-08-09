using InstitutoVirtus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoVirtus.Infrastructure.Data.Configurations;

public class ProfessorConfiguration : IEntityTypeConfiguration<Professor>
{
    public void Configure(EntityTypeBuilder<Professor> builder)
    {
        builder.Property(p => p.Especialidade)
            .HasMaxLength(100);

        builder.HasMany(p => p.Turmas)
            .WithOne(t => t.Professor)
            .HasForeignKey(t => t.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

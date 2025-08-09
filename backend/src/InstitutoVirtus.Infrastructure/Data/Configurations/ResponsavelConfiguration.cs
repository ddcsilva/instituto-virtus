using InstitutoVirtus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstitutoVirtus.Infrastructure.Data.Configurations;

public class ResponsavelConfiguration : IEntityTypeConfiguration<Responsavel>
{
    public void Configure(EntityTypeBuilder<Responsavel> builder)
    {
        builder.Property(r => r.SaldoCredito)
            .HasPrecision(10, 2)
            .HasDefaultValue(0);

        builder.HasMany(r => r.Pagamentos)
            .WithOne(p => p.Responsavel)
            .HasForeignKey(p => p.ResponsavelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

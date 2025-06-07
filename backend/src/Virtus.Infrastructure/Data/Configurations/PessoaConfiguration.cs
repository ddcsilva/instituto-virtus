using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Virtus.Domain.Entities;
using Virtus.Domain.ValueObjects;

namespace Virtus.Infrastructure.Data.Configurations;

public class PessoaConfiguration : IEntityTypeConfiguration<Pessoa>
{
  public void Configure(EntityTypeBuilder<Pessoa> builder)
  {
    builder.ToTable("Pessoas");

    builder.HasKey(p => p.Id);

    builder.Property(p => p.Id)
      .ValueGeneratedOnAdd();

    builder.Property(p => p.Nome)
      .IsRequired()
      .HasMaxLength(100);

    // Conversão de Value Object Email
    builder.Property(p => p.Email)
      .HasConversion(
        email => email != null ? email.Value : null,
        value => value != null ? new Email(value) : null)
      .HasMaxLength(150);

    // Conversão de Value Object Telefone
    builder.Property(p => p.Telefone)
      .HasConversion(
        telefone => telefone != null ? telefone.Value : null,
        value => value != null ? new Telefone(value) : null)
      .HasMaxLength(20);

    // Conversão de Value Object CPF
    builder.Property(p => p.CPF)
      .HasConversion(
        cpf => cpf != null ? cpf.Numero : null,
        value => value != null ? new CPF(value) : null)
      .HasMaxLength(11);

    builder.Property(p => p.Tipo)
      .IsRequired()
      .HasConversion<int>();

    builder.Property(p => p.Ativo)
      .IsRequired()
      .HasDefaultValue(true);

    builder.Property(p => p.CriadoEm)
      .IsRequired();

    builder.Property(p => p.AtualizadoEm);

    // Índices
    builder.HasIndex(p => p.CPF)
      .IsUnique()
      .HasFilter("[CPF] IS NOT NULL");

    builder.HasIndex(p => p.Email)
      .HasFilter("[Email] IS NOT NULL");

    builder.HasIndex(p => p.Tipo);
  }
}
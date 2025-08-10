namespace InstitutoVirtus.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InstitutoVirtus.Domain.Entities;
using InstitutoVirtus.Domain.Enums;

public class PessoaConfiguration : IEntityTypeConfiguration<Pessoa>
{
    public void Configure(EntityTypeBuilder<Pessoa> builder)
    {
        builder.ToTable("Pessoas");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.NomeCompleto)
            .IsRequired()
            .HasMaxLength(200);

        builder.OwnsOne(p => p.Telefone, telefone =>
        {
            telefone.Property(t => t.Numero)
                .HasColumnName("Telefone")
                .IsRequired()
                .HasMaxLength(11);

            telefone.HasIndex(t => t.Numero);
        });

        builder.OwnsOne(p => p.Email, email =>
        {
            email.Property(e => e.Endereco)
                .HasColumnName("Email")
                .HasMaxLength(150);

            email.HasIndex(e => e.Endereco);
        });

        builder.Property(p => p.DataNascimento)
            .IsRequired();

        builder.Property(p => p.Observacoes)
            .HasMaxLength(500);

        builder.Property(p => p.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        // Configuração de herança (TPH - Table Per Hierarchy)
        builder.HasDiscriminator(p => p.TipoPessoa)
            .HasValue<Pessoa>(TipoPessoa.Coordenador) // Valor para classe base
            .HasValue<Aluno>(TipoPessoa.Aluno)
            .HasValue<Professor>(TipoPessoa.Professor)
            .HasValue<Responsavel>(TipoPessoa.Responsavel);

        // Índices
        builder.HasIndex(p => p.TipoPessoa);
        builder.HasIndex(p => p.Ativo);
    }
}
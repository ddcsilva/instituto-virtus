using Virtus.Domain.Enums;
using Virtus.Domain.ValueObjects;

namespace Virtus.Domain.Entities;

public class Pessoa
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string? Telefone { get; private set; }
    public TipoPessoa Tipo { get; private set; }
    public DateTime DataCriacao { get; private set; }

    private Pessoa() { }

    public Pessoa(string nome, Email email, string? telefone, TipoPessoa tipo)
    {
        ValidarNome(nome);
        Nome = nome;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Telefone = telefone;
        Tipo = tipo;
        DataCriacao = DateTime.UtcNow;
    }

    public void AtualizarDados(string nome, Email email, string? telefone)
    {
        ValidarNome(nome);
        Nome = nome;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Telefone = telefone;
    }

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("Nome é obrigatório.", nameof(nome));
        }

        if (nome.Length < 3)
        {
            throw new ArgumentException("Nome deve ter pelo menos 3 caracteres.", nameof(nome));
        }

        if (nome.Length > 100)
        {
            throw new ArgumentException("Nome não pode ter mais de 100 caracteres.", nameof(nome));
        }
    }
}

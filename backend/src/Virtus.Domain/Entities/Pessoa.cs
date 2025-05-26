using Virtus.Domain.Enums;
using Virtus.Domain.ValueObjects;

namespace Virtus.Domain.Entities;

/// <summary>
/// Entidade que representa uma pessoa.
/// </summary>
public class Pessoa
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string Telefone { get; private set; } = string.Empty;
    public TipoPessoa Tipo { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime DataAtualizacao { get; private set; }

    private Pessoa() { }

    public Pessoa(string nome, Email email, string telefone, TipoPessoa tipo)
    {
        ValidarNome(nome);
        Nome = nome;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Telefone = telefone ?? throw new ArgumentNullException(nameof(telefone));
        Tipo = tipo;
        DataCriacao = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }

    /// <summary>
    /// Atualiza os dados da pessoa.
    /// </summary>
    /// <param name="nome">O novo nome da pessoa.</param>
    /// <param name="email">O novo email da pessoa.</param>
    /// <param name="telefone">O novo telefone da pessoa.</param>
    public void AtualizarDados(string nome, Email email, string telefone)
    {
        ValidarNome(nome);
        Nome = nome;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        ValidarTelefone(telefone);
        Telefone = telefone;
        DataAtualizacao = DateTime.UtcNow;
    }

    /// <summary>
    /// Altera o tipo de pessoa.
    /// </summary>
    /// <param name="novoTipo">O novo tipo de pessoa.</param>
    public void AlterarTipo(TipoPessoa novoTipo)
    {
        Tipo = novoTipo;
        DataAtualizacao = DateTime.UtcNow;
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

    private static void ValidarTelefone(string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
        {
            throw new ArgumentException("Telefone é obrigatório.", nameof(telefone));
        }
    }
}

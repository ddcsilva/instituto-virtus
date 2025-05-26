using System;
using System.Text.RegularExpressions;

namespace Virtus.Domain.ValueObjects;

/// <summary>
/// Value object que representa um endereço de e-mail.
/// </summary>
public sealed partial class Email : IEquatable<Email>
{
    public string Endereco { get; }

    private Email()
    {
        Endereco = string.Empty;
    }

    public Email(string endereco)
    {
        if (string.IsNullOrWhiteSpace(endereco))
        {
            throw new ArgumentException("O endereço de e-mail não pode ser vazio.", nameof(endereco));
        }

        if (!FormatoValido(endereco))
        {
            throw new ArgumentException("Endereço de e-mail em formato inválido.", nameof(endereco));
        }

        Endereco = endereco.Trim().ToLowerInvariant();
    }

    private static readonly Regex _regex = EmailRegex();

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    private static bool FormatoValido(string email) => _regex.IsMatch(email);

    public override bool Equals(object? obj) => Equals(obj as Email);

    public bool Equals(Email? outro) => outro is not null
        && string.Equals(Endereco, outro.Endereco, StringComparison.Ordinal);

    public override int GetHashCode() => Endereco.GetHashCode();

    public override string ToString() => Endereco;

    public static implicit operator string(Email email) => email?.Endereco ?? string.Empty;

    public static bool operator ==(Email? a, Email? b) => a is null ? b is null : a.Equals(b);

    public static bool operator !=(Email? a, Email? b) => !(a == b);

    /// <summary>
    /// Tenta criar um objeto Email a partir de um valor.
    /// </summary>
    /// <param name="valor">O valor a ser convertido para Email.</param>
    /// <param name="email">O objeto Email criado ou null se a conversão falhar.</param>
    /// <returns>true se a conversão for bem-sucedida, false caso contrário.</returns>
    public static bool TentarCriar(string valor, out Email? email)
    {
        email = null;
        if (string.IsNullOrWhiteSpace(valor) || !FormatoValido(valor))
        {
            return false;
        }

        email = new Email(valor);
        return true;
    }
}

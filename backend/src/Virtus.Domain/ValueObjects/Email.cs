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

        private Email(string endereco)
    {
        if (endereco is null)
        {
            throw new ArgumentException("O endereço de e-mail não pode ser vazio.", nameof(endereco));
        }

        var enderecoLimpo = endereco.Trim();

        if (string.IsNullOrWhiteSpace(enderecoLimpo) || !FormatoValido(enderecoLimpo))
        {
            throw new ArgumentException("Endereço de e-mail em formato inválido.", nameof(endereco));
        }

        Endereco = enderecoLimpo.ToLowerInvariant();
    }

    private static readonly Regex _regex = EmailRegex();

    [GeneratedRegex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
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
        if (string.IsNullOrWhiteSpace(valor))
        {
            return false;
        }

        var valorLimpo = valor.Trim();
        if (string.IsNullOrWhiteSpace(valorLimpo) || !FormatoValido(valorLimpo))
        {
            return false;
        }

        email = new Email(valor);
        return true;
    }

    /// <summary>
    /// Cria um novo objeto Email.
    /// </summary>
    /// <param name="endereco">O endereço de e-mail a ser criado.</param>
    /// <returns>O objeto Email criado.</returns>
    public static Email Criar(string endereco)
    {
        return new Email(endereco);
    }

}

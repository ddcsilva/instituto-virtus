using System.Text.RegularExpressions;
using Virtus.Domain.Exceptions;

namespace Virtus.Domain.ValueObjects;

/// <summary>
/// Value Object para representar um endereço de email
/// </summary>
public class Email : IEquatable<Email>
{
  private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

  public string Value { get; }

  public Email(string email)
  {
    if (string.IsNullOrWhiteSpace(email))
      throw new ValidationException("Email não pode ser vazio");

    if (!IsValid(email))
      throw new ValidationException("Email inválido");

    Value = email.ToLowerInvariant();
  }

  /// <summary>
  /// Valida o formato do email
  /// </summary>
  public static bool IsValid(string email)
  {
    return !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);
  }

  public override string ToString() => Value;

  public override bool Equals(object? obj) => Equals(obj as Email);

  public bool Equals(Email? other)
  {
    if (other is null) return false;
    if (ReferenceEquals(this, other)) return true;
    return Value == other.Value;
  }

  public override int GetHashCode() => Value.GetHashCode();

  public static bool operator ==(Email? left, Email? right) => Equals(left, right);
  public static bool operator !=(Email? left, Email? right) => !Equals(left, right);

  // Conversão implícita de string para Email
  public static implicit operator string(Email email) => email.Value;
}
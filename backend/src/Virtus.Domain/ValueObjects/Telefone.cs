using System.Text.RegularExpressions;
using Virtus.Domain.Exceptions;

namespace Virtus.Domain.ValueObjects;

/// <summary>
/// Value Object para representar um número de telefone brasileiro
/// </summary>
public class Telefone : IEquatable<Telefone>
{
  // Aceita formatos: (11) 98765-4321, 11987654321, (11) 3456-7890
  private static readonly Regex TelefoneRegex = new(
    @"^\(?([1-9]{2})\)?\s?([9]?[0-9]{4})-?([0-9]{4})$",
    RegexOptions.Compiled
  );

  public string Value { get; }
  public string Numero { get; }
  public string DDD { get; }

  public Telefone(string telefone)
  {
    if (string.IsNullOrWhiteSpace(telefone))
      throw new ValidationException("Telefone não pode ser vazio");

    var numeroLimpo = Regex.Replace(telefone, @"[^\d]", "");

    if (!IsValid(telefone))
      throw new ValidationException("Telefone inválido");

    var match = TelefoneRegex.Match(telefone);
    DDD = match.Groups[1].Value;
    Numero = match.Groups[2].Value + match.Groups[3].Value;

    // Formato padrão: (11) 98765-4321
    Value = $"({DDD}) {Numero.Substring(0, Numero.Length - 4)}-{Numero.Substring(Numero.Length - 4)}";
  }

  /// <summary>
  /// Valida o formato do telefone brasileiro
  /// </summary>
  public static bool IsValid(string telefone)
  {
    if (string.IsNullOrWhiteSpace(telefone)) return false;
    return TelefoneRegex.IsMatch(telefone);
  }

  public override string ToString() => Value;

  public override bool Equals(object? obj) => Equals(obj as Telefone);

  public bool Equals(Telefone? other)
  {
    if (other is null) return false;
    if (ReferenceEquals(this, other)) return true;
    return DDD + Numero == other.DDD + other.Numero;
  }

  public override int GetHashCode() => (DDD + Numero).GetHashCode();

  public static bool operator ==(Telefone? left, Telefone? right) => Equals(left, right);
  public static bool operator !=(Telefone? left, Telefone? right) => !Equals(left, right);
}
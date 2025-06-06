using System.Text.RegularExpressions;
using Virtus.Domain.Exceptions;

namespace Virtus.Domain.ValueObjects;

/// <summary>
/// Value Object para representar um CPF brasileiro
/// </summary>
public class CPF : IEquatable<CPF>
{
  private static readonly Regex CPFRegex = new(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$|^\d{11}$", RegexOptions.Compiled);

  public string Value { get; }
  public string Numero { get; }

  public CPF(string cpf)
  {
    if (string.IsNullOrWhiteSpace(cpf))
      throw new ValidationException("CPF não pode ser vazio");

    var numeroLimpo = Regex.Replace(cpf, @"[^\d]", "");

    if (!IsValid(numeroLimpo))
      throw new ValidationException("CPF inválido");

    Numero = numeroLimpo;
    Value = $"{numeroLimpo.Substring(0, 3)}.{numeroLimpo.Substring(3, 3)}.{numeroLimpo.Substring(6, 3)}-{numeroLimpo.Substring(9, 2)}";
  }

  /// <summary>
  /// Valida o CPF usando algoritmo oficial
  /// </summary>
  public static bool IsValid(string cpf)
  {
    if (string.IsNullOrWhiteSpace(cpf)) return false;

    var numeroLimpo = Regex.Replace(cpf, @"[^\d]", "");

    if (numeroLimpo.Length != 11) return false;

    if (numeroLimpo.Distinct().Count() == 1) return false;

    var soma = 0;
    for (var i = 0; i < 9; i++)
      soma += int.Parse(numeroLimpo[i].ToString()) * (10 - i);

    var resto = soma % 11;
    var primeiroDigito = resto < 2 ? 0 : 11 - resto;

    if (int.Parse(numeroLimpo[9].ToString()) != primeiroDigito)
      return false;

    soma = 0;
    for (var i = 0; i < 10; i++)
      soma += int.Parse(numeroLimpo[i].ToString()) * (11 - i);

    resto = soma % 11;
    var segundoDigito = resto < 2 ? 0 : 11 - resto;

    return int.Parse(numeroLimpo[10].ToString()) == segundoDigito;
  }

  public override string ToString() => Value;

  public override bool Equals(object? obj) => Equals(obj as CPF);

  public bool Equals(CPF? other)
  {
    if (other is null) return false;
    if (ReferenceEquals(this, other)) return true;
    return Numero == other.Numero;
  }

  public override int GetHashCode() => Numero.GetHashCode();

  public static bool operator ==(CPF? left, CPF? right) => Equals(left, right);
  public static bool operator !=(CPF? left, CPF? right) => !Equals(left, right);
}
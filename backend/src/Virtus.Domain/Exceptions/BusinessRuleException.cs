namespace Virtus.Domain.Exceptions;

/// <summary>
/// Exceção para violação de regras de negócio
/// </summary>
public class BusinessRuleException : DomainException
{
  public string RuleName { get; }

  public BusinessRuleException(string message, string? ruleName = null) : base(message)
  {
    RuleName = ruleName ?? "UnknownRule";
  }

  public BusinessRuleException(string message, string ruleName, Exception innerException) : base(message, innerException)
  {
    RuleName = ruleName;
  }
}
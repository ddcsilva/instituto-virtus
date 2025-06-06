namespace Virtus.Domain.Exceptions;

/// <summary>
/// Exceção para erros de validação
/// </summary>
public class ValidationException : DomainException
{
  public string PropertyName { get; }
  public object? AttemptedValue { get; }

  public ValidationException(string message, string? propertyName = null, object? attemptedValue = null) : base(message)
  {
    PropertyName = propertyName ?? "Unknown";
    AttemptedValue = attemptedValue;
  }

  public ValidationException(string message, Exception innerException) : base(message, innerException)
  {
    PropertyName = "Unknown";
  }
}
namespace Virtus.Domain.Exceptions;

/// <summary>
/// Exceção base para erros de domínio
/// </summary>
public class DomainException : Exception
{
  public DomainException(string message) : base(message) { }

  public DomainException(string message, Exception innerException) : base(message, innerException) { }
}
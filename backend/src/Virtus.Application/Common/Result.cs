namespace Virtus.Application.Common;

/// <summary>
/// Classe genérica para retornar resultados de operações.
/// </summary>
/// <typeparam name="T">O tipo do valor retornado.</typeparam>
public class Result<T>
{
  public bool IsSuccess { get; }
  public T? Value { get; }
  public string? Error { get; }

  private Result(bool isSuccess, T? value, string? error)
  {
    IsSuccess = isSuccess;
    Value = value;
    Error = error;
  }

  public static Result<T> Success(T value) => new(true, value, null);
  public static Result<T> Failure(string error) => new(false, default, error);
}
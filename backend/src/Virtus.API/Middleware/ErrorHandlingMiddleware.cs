using System.Net;
using System.Text.Json;

namespace Virtus.API.Middleware;

/// <summary>
/// Middleware para tratamento de erros.
/// </summary>
public class ErrorHandlingMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<ErrorHandlingMiddleware> _logger;

  public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  /// <summary>
  /// Invoca o middleware para tratamento de erros.
  /// </summary>
  /// <param name="context">O contexto da requisição.</param>
  /// <returns>Uma tarefa que representa o tratamento da requisição.</returns>
  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Erro não tratado");
      await HandleExceptionAsync(context, ex);
    }
  }

  /// <summary>
  /// Lida com exceções não tratadas.
  /// </summary>
  /// <param name="context">O contexto da requisição.</param>
  /// <param name="exception">A exceção não tratada.</param>
  /// <returns>Uma tarefa que representa o tratamento da exceção.</returns>
  private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
  {
    context.Response.ContentType = "application/json";
    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

    var response = new
    {
      status = context.Response.StatusCode,
      message = "Ocorreu um erro ao processar sua requisição.",
      details = exception.Message
    };

    var jsonResponse = JsonSerializer.Serialize(response);
    await context.Response.WriteAsync(jsonResponse);
  }
}
namespace InstitutoVirtus.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = Guid.NewGuid().ToString();

        context.Response.Headers.Add("X-Request-Id", requestId);

        _logger.LogInformation(
            "Request {RequestId} - {Method} {Path} - User: {User}",
            requestId,
            context.Request.Method,
            context.Request.Path,
            context.User?.Identity?.Name ?? "Anonymous");

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            _logger.LogInformation(
                "Request {RequestId} completed - Status: {StatusCode} - Duration: {Duration}ms",
                requestId,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
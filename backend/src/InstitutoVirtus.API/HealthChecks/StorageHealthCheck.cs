namespace InstitutoVirtus.API.HealthChecks;

using System.Text;
using InstitutoVirtus.Infrastructure.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public class StorageHealthCheck : IHealthCheck
{
    private readonly IStorageService _storageService;

    public StorageHealthCheck(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Verificar se o storage está acessível
            var testFileName = $"health_check_{Guid.NewGuid()}.txt";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("Health Check"));

            var url = _storageService.UploadFileAsync(stream, testFileName, "text/plain", cancellationToken).Result;
            _storageService.DeleteFileAsync(url, cancellationToken).Wait();

            return Task.FromResult(HealthCheckResult.Healthy("Storage está funcionando"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"Storage com problema: {ex.Message}"));
        }
    }
}

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

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var testFileName = $"health_check_{Guid.NewGuid()}.txt";
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("Health Check"));

            var url = await _storageService.UploadFileAsync(stream, testFileName, "text/plain", cancellationToken);
            await _storageService.DeleteFileAsync(url, cancellationToken);

            return HealthCheckResult.Healthy("Storage está funcionando");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Storage com problema: {ex.Message}");
        }
    }
}

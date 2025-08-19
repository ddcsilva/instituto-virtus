using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InstitutoVirtus.Infrastructure.Services;

public interface IStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<Stream> DownloadFileAsync(string fileUrl, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
}

public class StorageService : IStorageService
{
    private readonly string _basePath;
    private readonly ILogger<StorageService> _logger;

    public StorageService(IConfiguration configuration, ILogger<StorageService> logger)
    {
        _basePath = configuration["Storage:BasePath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        _logger = logger;

        if (!Directory.Exists(_basePath))
            Directory.CreateDirectory(_basePath);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(_basePath, uniqueFileName);

        using var fileOutput = File.Create(filePath);
        await fileStream.CopyToAsync(fileOutput, cancellationToken);

        _logger.LogInformation($"Arquivo salvo: {filePath}");
        return $"/uploads/{uniqueFileName}";
    }

    public Task<Stream> DownloadFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        var fileName = Path.GetFileName(fileUrl);
        var filePath = Path.Combine(_basePath, fileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Arquivo não encontrado: {filePath}");

        return Task.FromResult<Stream>(new FileStream(filePath, FileMode.Open, FileAccess.Read));
    }

    public Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        var fileName = Path.GetFileName(fileUrl);
        var filePath = Path.Combine(_basePath, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            _logger.LogInformation($"Arquivo deletado: {filePath}");
        }

        return Task.CompletedTask;
    }
}

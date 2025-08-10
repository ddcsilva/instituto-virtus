using InstitutoVirtus.Infrastructure.Data.Context;

namespace InstitutoVirtus.API.Extensions;

public static class HostExtensions
{
    public static async Task<IHost> SeedDataAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var dbContext = services.GetRequiredService<VirtusDbContext>();
            await SeedData.InitializeAsync(dbContext);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Erro ao fazer seed dos dados");
        }

        return host;
    }
}

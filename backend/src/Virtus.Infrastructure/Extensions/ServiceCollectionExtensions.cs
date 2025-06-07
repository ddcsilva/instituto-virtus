using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Virtus.Domain.Interfaces;
using Virtus.Infrastructure.Data;
using Virtus.Infrastructure.Repositories;
using Virtus.Infrastructure.Services;

namespace Virtus.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    // Configuração do Entity Framework
    services.AddDbContext<VirtusDbContext>(options =>
    {
      options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));

      // Configuração de logging em desenvolvimento
      var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
      if (isDevelopment)
      {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
        options.LogTo(Console.WriteLine, LogLevel.Information);
      }
    });

    // Registro dos repositórios
    services.AddScoped<IAlunoRepository, AlunoRepository>();
    services.AddScoped<IProfessorRepository, ProfessorRepository>();
    services.AddScoped<ITurmaRepository, TurmaRepository>();
    services.AddScoped<IMatriculaRepository, MatriculaRepository>();
    services.AddScoped<IPagamentoRepository, PagamentoRepository>();

    // Registro do Unit of Work
    services.AddScoped<IUnitOfWork, UnitOfWork>();

    return services;
  }

  public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
  {
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<VirtusDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<VirtusDbContext>>();

    try
    {
      logger.LogInformation("Aplicando migrações do banco de dados...");
      await context.Database.MigrateAsync();
      logger.LogInformation("Migrações aplicadas com sucesso");

      // Aplica seed data em desenvolvimento
      var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
      if (isDevelopment)
      {
        logger.LogInformation("Aplicando seed data de desenvolvimento...");
        await Data.SeedData.DevelopmentSeedData.SeedAsync(context);
        logger.LogInformation("Seed data aplicado com sucesso");
      }
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Erro ao aplicar migrações");
      throw;
    }
  }
}
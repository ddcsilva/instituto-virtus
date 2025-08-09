namespace InstitutoVirtus.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using InstitutoVirtus.Infrastructure.Data;
using InstitutoVirtus.Infrastructure.Data.Context;
using InstitutoVirtus.Infrastructure.Data.Repositories;
using InstitutoVirtus.Infrastructure.Services;
using InstitutoVirtus.Domain.Interfaces;
using InstitutoVirtus.Domain.Interfaces.Repositories;
using InstitutoVirtus.Application.Common.Interfaces;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<VirtusDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (configuration["DatabaseProvider"] == "Sqlite")
            {
                options.UseSqlite(connectionString);
            }
            else
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(VirtusDbContext).Assembly.FullName);
                });
            }

            if (configuration.GetValue<bool>("EnableSensitiveDataLogging"))
            {
                options.EnableSensitiveDataLogging();
            }
        });

        // Repositories
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IPessoaRepository, PessoaRepository>();
        services.AddScoped<ICursoRepository, CursoRepository>();
        services.AddScoped<ITurmaRepository, TurmaRepository>();
        services.AddScoped<IMatriculaRepository, MatriculaRepository>();
        services.AddScoped<IMensalidadeRepository, MensalidadeRepository>();
        services.AddScoped<IPagamentoRepository, PagamentoRepository>();
        services.AddScoped<IAulaRepository, AulaRepository>();
        services.AddScoped<IAvaliacaoRepository, AvaliacaoRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IStorageService, StorageService>();

        return services;
    }
}
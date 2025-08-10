using HealthChecks.UI.Client;
using InstitutoVirtus.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace InstitutoVirtus.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger(options =>
        {
            options.SerializeAsV2 = false;
        });

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Instituto Virtus API V1");
            options.RoutePrefix = env.IsDevelopment() ? string.Empty : "swagger";
            options.DocumentTitle = "Instituto Virtus API Documentation";
            options.DisplayRequestDuration();
            options.EnableFilter();
            options.EnableDeepLinking();
            options.ShowExtensions();
            options.EnableValidator();
            options.DefaultModelsExpandDepth(0);
        });

        return app;
    }

    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/error");
            app.UseHsts();
        }

        return app;
    }

    public static IApplicationBuilder UseCustomHealthChecks(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseHealthChecksUI(options =>
        {
            options.UIPath = "/health-ui";
            options.ApiPath = "/health-ui-api";
        });

        return app;
    }

    public static async Task<IApplicationBuilder> ApplyMigrationsAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VirtusDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Aplicando migrations...");
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Migrations aplicadas com sucesso");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao aplicar migrations");
            throw;
        }

        return app;
    }
}

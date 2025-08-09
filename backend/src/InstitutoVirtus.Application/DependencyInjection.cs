namespace InstitutoVirtus.Application;

using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using MediatR;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}

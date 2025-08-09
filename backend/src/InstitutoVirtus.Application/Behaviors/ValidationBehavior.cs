namespace InstitutoVirtus.Application.Behaviors;

using FluentValidation;
using InstitutoVirtus.Application.Common;
using MediatR;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                var errors = failures.Select(f => f.ErrorMessage).ToList();

                // Se TResponse for Result<T>, retorna failure com erros
                if (typeof(TResponse).IsGenericType &&
                    typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var resultType = typeof(TResponse).GetGenericArguments()[0];
                    var failureMethod = typeof(Result<>)
                        .MakeGenericType(resultType)
                        .GetMethod("Failure", new[] { typeof(List<string>) });

                    return (TResponse)failureMethod!.Invoke(null, new object[] { errors })!;
                }

                // Se TResponse for Result, retorna failure com erros
                if (typeof(TResponse) == typeof(Result))
                {
                    return (TResponse)(object)Result.Failure(errors);
                }

                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}

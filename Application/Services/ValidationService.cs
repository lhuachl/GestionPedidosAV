using FluentValidation;
using GestionPedidosAV.Application.Interfaces;

namespace GestionPedidosAV.Application.Services;

public class ValidationService : IValidationService
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<(bool IsValid, IEnumerable<string> Errors)> ValidateAsync<T>(T model)
    {
        var validator = _serviceProvider.GetService<IValidator<T>>();
        
        if (validator == null)
        {
            return (true, Enumerable.Empty<string>());
        }

        var validationResult = await validator.ValidateAsync(model);
        
        return (validationResult.IsValid, validationResult.Errors.Select(e => e.ErrorMessage));
    }
}
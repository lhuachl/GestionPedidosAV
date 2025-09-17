namespace GestionPedidosAV.Application.Interfaces;

public interface IValidationService
{
    Task<(bool IsValid, IEnumerable<string> Errors)> ValidateAsync<T>(T model);
}
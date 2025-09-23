using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace GestionPedidosAV.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    public string? ErrorMessage { get; set; }
    public int StatusCode { get; set; }

    private readonly ILogger<ErrorModel> _logger;

    public ErrorModel(ILogger<ErrorModel> logger)
    {
        _logger = logger;
    }

    public void OnGet(int? statusCode = null)
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        StatusCode = statusCode ?? 500;

        switch (StatusCode)
        {
            case 404:
                ErrorMessage = "La página que buscas no existe.";
                break;
            case 403:
                ErrorMessage = "No tienes permisos para acceder a este recurso.";
                break;
            case 401:
                ErrorMessage = "Necesitas iniciar sesión para acceder a esta página.";
                break;
            case 500:
                ErrorMessage = "Ha ocurrido un error interno en el servidor.";
                break;
            default:
                ErrorMessage = "Ha ocurrido un error inesperado.";
                break;
        }

        _logger.LogError("Error {StatusCode} occurred. RequestId: {RequestId}", StatusCode, RequestId);
    }
}
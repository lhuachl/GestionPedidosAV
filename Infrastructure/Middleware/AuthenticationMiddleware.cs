namespace GestionPedidosAV.Infrastructure.Middleware;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";
        
        // Rutas que no requieren autenticaci�n
        var publicPaths = new[]
        {
            "/account/login",
            "/account/register",
            "/css/",
            "/js/",
            "/lib/",
            "/favicon.ico"
        };

        var isPublicPath = publicPaths.Any(p => path.StartsWith(p));
        
        // Si es una ruta p�blica, continuar
        if (isPublicPath)
        {
            await _next(context);
            return;
        }

        // Verificar si el usuario est� autenticado
        var userId = context.Session.GetString("UserId");
        
        if (string.IsNullOrEmpty(userId))
        {
            // Redirigir al login si no est� autenticado
            var returnUrl = context.Request.Path + context.Request.QueryString;
            context.Response.Redirect($"/Account/Login?returnUrl={Uri.EscapeDataString(returnUrl)}");
            return;
        }

        await _next(context);
    }
}
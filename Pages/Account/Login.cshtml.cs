using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GestionPedidosAV.Application.Interfaces;

namespace GestionPedidosAV.Pages.Account;

public class LoginModel : PageModel
{
    private readonly IUserService _userService;

    public LoginModel(IUserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public bool RememberMe { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public void OnGet()
    {
        // Check if user is already logged in
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
        {
            Response.Redirect("/");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            // Validate credentials
            if (await _userService.ValidateUserCredentialsAsync(Email, Password))
            {
                var user = await _userService.GetUserByEmailAsync(Email);
                if (user != null && user.IsActive)
                {
                    // Create session
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("UserName", user.FullName);
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("UserRole", user.Role.ToString());

                    TempData["SuccessMessage"] = $"¡Bienvenido, {user.FullName}!";

                    // Redirect to return URL or home
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }

                    return RedirectToPage("/Index");
                }
                else
                {
                    ModelState.AddModelError("", "La cuenta está inactiva. Contacta al administrador.");
                    return Page();
                }
            }
            else
            {
                ModelState.AddModelError("", "Email o contraseña incorrectos.");
                return Page();
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al iniciar sesión: {ex.Message}");
            return Page();
        }
    }
}
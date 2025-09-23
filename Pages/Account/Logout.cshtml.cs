using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestionPedidosAV.Pages.Account;

public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        return OnPostAsync();
    }

    public IActionResult OnPostAsync()
    {
        // Clear all session data
        HttpContext.Session.Clear();
        
        TempData["SuccessMessage"] = "Has cerrado sesión correctamente";
        
        return RedirectToPage("/Account/Login");
    }
}
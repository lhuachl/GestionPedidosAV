using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GestionPedidosAV.Application.Interfaces;
using GestionPedidosAV.Application.DTOs;
using GestionPedidosAV.Domain.Enums;

namespace GestionPedidosAV.Pages.Users;

public class CreateModel : PageModel
{
    private readonly IUserService _userService;

    public CreateModel(IUserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public CreateUserDto User { get; set; } = new();

    public void OnGet()
    {
        // Initialize with default values if needed
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            // Check if email already exists
            if (await _userService.EmailExistsAsync(User.Email))
            {
                ModelState.AddModelError("User.Email", "Este email ya está registrado en el sistema");
                return Page();
            }

            var createdUser = await _userService.CreateUserAsync(User);
            TempData["SuccessMessage"] = $"Usuario '{createdUser.FullName}' creado correctamente";
            
            return RedirectToPage("./Index");
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return Page();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al crear usuario: {ex.Message}");
            return Page();
        }
    }
}
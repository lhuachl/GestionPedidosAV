using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GestionPedidosAV.Application.Interfaces;
using GestionPedidosAV.Application.DTOs;

namespace GestionPedidosAV.Pages.Users;

public class EditModel : PageModel
{
    private readonly IUserService _userService;

    public EditModel(IUserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public UpdateUserDto User { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Usuario no encontrado";
                return RedirectToPage("./Index");
            }

            User = new UpdateUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                IsActive = user.IsActive
            };

            return Page();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al cargar usuario: {ex.Message}";
            return RedirectToPage("./Index");
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
            // Check if email is being changed and already exists
            var existingUser = await _userService.GetUserByIdAsync(User.Id);
            if (existingUser != null && existingUser.Email != User.Email)
            {
                if (await _userService.EmailExistsAsync(User.Email))
                {
                    ModelState.AddModelError("User.Email", "Este email ya está registrado en el sistema");
                    return Page();
                }
            }

            var updatedUser = await _userService.UpdateUserAsync(User);
            TempData["SuccessMessage"] = $"Usuario '{updatedUser.FullName}' actualizado correctamente";
            
            return RedirectToPage("./Details", new { id = User.Id });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return Page();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al actualizar usuario: {ex.Message}");
            return Page();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GestionPedidosAV.Application.Interfaces;
using GestionPedidosAV.Application.DTOs;

namespace GestionPedidosAV.Pages.Users;

public class IndexModel : PageModel
{
    private readonly IUserService _userService;

    public IndexModel(IUserService userService)
    {
        _userService = userService;
    }

    public List<UserDto> Users { get; set; } = new();
    
    [BindProperty(SupportsGet = true)]
    public string SearchTerm { get; set; } = string.Empty;
    
    [BindProperty(SupportsGet = true)]
    public string RoleFilter { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        try
        {
            var allUsers = await _userService.GetAllUsersAsync();
            Users = allUsers.ToList();

            // Apply filters
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                Users = Users.Where(u => 
                    u.FullName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (!string.IsNullOrEmpty(RoleFilter))
            {
                if (Enum.TryParse<Domain.Enums.UserRole>(RoleFilter, out var role))
                {
                    Users = Users.Where(u => u.Role == role).ToList();
                }
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al cargar usuarios: {ex.Message}";
            Users = new List<UserDto>();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            TempData["SuccessMessage"] = "Usuario eliminado correctamente";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al eliminar usuario: {ex.Message}";
        }

        return RedirectToPage();
    }
}
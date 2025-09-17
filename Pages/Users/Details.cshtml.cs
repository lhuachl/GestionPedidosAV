using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GestionPedidosAV.Application.Interfaces;
using GestionPedidosAV.Application.DTOs;

namespace GestionPedidosAV.Pages.Users;

public class DetailsModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IOrderService _orderService;

    public DetailsModel(IUserService userService, IOrderService orderService)
    {
        _userService = userService;
        _orderService = orderService;
    }

    public UserDto User { get; set; } = new();
    public List<OrderDto> UserOrders { get; set; } = new();
    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }

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

            User = user;

            // Get user orders
            var orders = await _orderService.GetOrdersByUserIdAsync(id);
            UserOrders = orders.Take(10).ToList(); // Latest 10 orders
            TotalOrders = orders.Count();
            TotalSpent = orders.Sum(o => o.Total);

            return Page();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al cargar usuario: {ex.Message}";
            return RedirectToPage("./Index");
        }
    }
}
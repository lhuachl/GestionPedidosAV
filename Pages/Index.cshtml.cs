using Microsoft.AspNetCore.Mvc.RazorPages;
using GestionPedidosAV.Application.Interfaces;
using GestionPedidosAV.Application.DTOs;
using GestionPedidosAV.Domain.Enums;

namespace GestionPedidosAV.Pages;

public class IndexModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IProductService _productService;
    private readonly IOrderService _orderService;

    public IndexModel(IUserService userService, IProductService productService, IOrderService orderService)
    {
        _userService = userService;
        _productService = productService;
        _orderService = orderService;
    }

    public int TotalUsers { get; set; }
    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public List<OrderDto> RecentOrders { get; set; } = new();
    public List<ProductDto> LowStockProducts { get; set; } = new();

    public async Task OnGetAsync()
    {
        // Get statistics
        var users = await _userService.GetAllUsersAsync();
        TotalUsers = users.Count();

        var products = await _productService.GetAllProductsAsync();
        TotalProducts = products.Count();

        var orders = await _orderService.GetAllOrdersAsync();
        TotalOrders = orders.Count();

        var pendingOrders = await _orderService.GetOrdersByStatusAsync(OrderStatus.Pendiente);
        PendingOrders = pendingOrders.Count();

        // Get recent orders (last 10)
        RecentOrders = orders.OrderByDescending(o => o.OrderDate).Take(10).ToList();

        // Get low stock products (stock <= 5)
        LowStockProducts = products.Where(p => p.Stock <= 5 && p.IsAvailable).ToList();
    }
}
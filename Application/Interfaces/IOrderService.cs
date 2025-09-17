using GestionPedidosAV.Application.DTOs;
using GestionPedidosAV.Domain.Enums;

namespace GestionPedidosAV.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto?> GetOrderByIdAsync(int id);
    Task<OrderDto?> GetOrderWithDetailsAsync(int id);
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId);
    Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status);
    Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
    Task<OrderDto> UpdateOrderStatusAsync(UpdateOrderStatusDto updateOrderStatusDto);
    Task DeleteOrderAsync(int id);
    Task<bool> CanProcessOrderAsync(CreateOrderDto createOrderDto);
}
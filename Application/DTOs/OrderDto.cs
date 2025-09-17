using GestionPedidosAV.Domain.Enums;

namespace GestionPedidosAV.Application.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Tax { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Total { get; set; }
    public string? ShippingAddress { get; set; }
    public string? Notes { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public string? TrackingNumber { get; set; }
    public UserDto? User { get; set; }
    public List<OrderItemDto> OrderItems { get; set; } = new();
}

public class CreateOrderDto
{
    public int UserId { get; set; }
    public string? ShippingAddress { get; set; }
    public string? Notes { get; set; }
    public decimal ShippingCost { get; set; }
    public List<CreateOrderItemDto> OrderItems { get; set; } = new();
}

public class UpdateOrderStatusDto
{
    public int Id { get; set; }
    public OrderStatus Status { get; set; }
    public string? TrackingNumber { get; set; }
    public string? Notes { get; set; }
}

public class OrderItemDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }
    public string? Notes { get; set; }
    public ProductDto? Product { get; set; }
}

public class CreateOrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
}
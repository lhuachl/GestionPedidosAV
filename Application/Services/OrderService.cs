using AutoMapper;
using GestionPedidosAV.Application.DTOs;
using GestionPedidosAV.Application.Interfaces;
using GestionPedidosAV.Domain.Entities;
using GestionPedidosAV.Domain.Enums;
using GestionPedidosAV.Domain.Interfaces;

namespace GestionPedidosAV.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidationService _validationService;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IValidationService validationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationService = validationService;
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(id);
        return order != null ? _mapper.Map<OrderDto>(order) : null;
    }

    public async Task<OrderDto?> GetOrderWithDetailsAsync(int id)
    {
        var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(id);
        return order != null ? _mapper.Map<OrderDto>(order) : null;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _unitOfWork.Orders.GetAllAsync();
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId)
    {
        var orders = await _unitOfWork.Orders.GetOrdersByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status)
    {
        var orders = await _unitOfWork.Orders.GetOrdersByStatusAsync(status);
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
    {
        var validation = await _validationService.ValidateAsync(createOrderDto);
        if (!validation.IsValid)
        {
            throw new ArgumentException(string.Join(", ", validation.Errors));
        }

        // Validate stock availability
        if (!await CanProcessOrderAsync(createOrderDto))
        {
            throw new InvalidOperationException("Stock insuficiente para uno o más productos");
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Create order
            var order = _mapper.Map<Order>(createOrderDto);
            order.OrderNumber = await _unitOfWork.Orders.GenerateOrderNumberAsync();
            order.OrderDate = DateTime.UtcNow;
            order.Status = OrderStatus.Pendiente;
            order.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            // Create order items and update stock
            foreach (var itemDto in createOrderDto.OrderItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(itemDto.ProductId);
                if (product == null)
                {
                    throw new ArgumentException($"Producto con ID {itemDto.ProductId} no encontrado");
                }

                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price,
                    Notes = itemDto.Notes
                };
                orderItem.CalculateSubTotal();

                await _unitOfWork.Repository<OrderItem>().AddAsync(orderItem);

                // Update product stock
                await _unitOfWork.Products.UpdateStockAsync(itemDto.ProductId, itemDto.Quantity);
            }

            await _unitOfWork.SaveChangesAsync();

            // Recalculate order totals
            var orderWithItems = await _unitOfWork.Orders.GetOrderWithDetailsAsync(order.Id);
            if (orderWithItems != null)
            {
                orderWithItems.CalculateTotal();
                await _unitOfWork.Orders.UpdateAsync(orderWithItems);
                await _unitOfWork.SaveChangesAsync();
            }

            await _unitOfWork.CommitTransactionAsync();

            return _mapper.Map<OrderDto>(orderWithItems);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<OrderDto> UpdateOrderStatusAsync(UpdateOrderStatusDto updateOrderStatusDto)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(updateOrderStatusDto.Id);
        if (order == null)
        {
            throw new ArgumentException("Pedido no encontrado");
        }

        order.Status = updateOrderStatusDto.Status;
        order.TrackingNumber = updateOrderStatusDto.TrackingNumber;
        order.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(updateOrderStatusDto.Notes))
        {
            order.Notes = updateOrderStatusDto.Notes;
        }

        // Update specific dates based on status
        switch (updateOrderStatusDto.Status)
        {
            case OrderStatus.Enviado:
                order.ShippedDate = DateTime.UtcNow;
                break;
            case OrderStatus.Entregado:
                order.DeliveredDate = DateTime.UtcNow;
                break;
        }

        await _unitOfWork.Orders.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<OrderDto>(order);
    }

    public async Task DeleteOrderAsync(int id)
    {
        var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(id);
        if (order == null)
        {
            throw new ArgumentException("Pedido no encontrado");
        }

        // Only allow deletion if order is still pending
        if (order.Status != OrderStatus.Pendiente)
        {
            throw new InvalidOperationException("Solo se pueden eliminar pedidos pendientes");
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Restore stock for all items
            foreach (var item in order.OrderItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock += item.Quantity;
                    product.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.Products.UpdateAsync(product);
                }
            }

            // Soft delete the order
            order.IsDeleted = true;
            order.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Orders.UpdateAsync(order);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> CanProcessOrderAsync(CreateOrderDto createOrderDto)
    {
        foreach (var item in createOrderDto.OrderItems)
        {
            if (!await _unitOfWork.Products.IsStockAvailableAsync(item.ProductId, item.Quantity))
            {
                return false;
            }
        }
        return true;
    }
}
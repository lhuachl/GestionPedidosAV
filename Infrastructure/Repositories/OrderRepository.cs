using Microsoft.EntityFrameworkCore;
using GestionPedidosAV.Domain.Entities;
using GestionPedidosAV.Domain.Enums;
using GestionPedidosAV.Domain.Interfaces;
using GestionPedidosAV.Infrastructure.Data;

namespace GestionPedidosAV.Infrastructure.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.UserId == userId && !o.IsDeleted)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
    {
        return await _dbSet
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.Status == status && !o.IsDeleted)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
    {
        return await _dbSet
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);
    }

    public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && !o.IsDeleted)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<string> GenerateOrderNumberAsync()
    {
        var lastOrder = await _dbSet
            .OrderByDescending(o => o.Id)
            .FirstOrDefaultAsync();

        var nextNumber = (lastOrder?.Id ?? 0) + 1;
        return $"ORD-{DateTime.UtcNow:yyyyMM}-{nextNumber:D6}";
    }

    public override async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _dbSet
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => !o.IsDeleted)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public override async Task<Order?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
    }
}
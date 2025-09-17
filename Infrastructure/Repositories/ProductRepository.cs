using Microsoft.EntityFrameworkCore;
using GestionPedidosAV.Domain.Entities;
using GestionPedidosAV.Domain.Interfaces;
using GestionPedidosAV.Infrastructure.Data;

namespace GestionPedidosAV.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
    {
        return await _dbSet
            .Where(p => p.Category == category && !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _dbSet
            .Where(p => p.Price >= minPrice && p.Price <= maxPrice && !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        return await _dbSet
            .Where(p => (p.Name.Contains(searchTerm) || 
                        p.Description!.Contains(searchTerm) || 
                        p.Category!.Contains(searchTerm)) && !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<bool> IsStockAvailableAsync(int productId, int quantity)
    {
        var product = await GetByIdAsync(productId);
        return product != null && product.Stock >= quantity && product.IsAvailable;
    }

    public async Task UpdateStockAsync(int productId, int quantity)
    {
        var product = await GetByIdAsync(productId);
        if (product != null)
        {
            product.Stock -= quantity;
            product.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(product);
        }
    }

    public async Task<IEnumerable<Product>> GetAvailableProductsAsync()
    {
        return await _dbSet
            .Where(p => p.Status == Domain.Enums.ProductStatus.Active && 
                       p.Stock > 0 && !p.IsDeleted)
            .ToListAsync();
    }

    public override async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _dbSet.Where(p => !p.IsDeleted).ToListAsync();
    }

    public override async Task<Product?> GetByIdAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }
}
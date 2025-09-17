using GestionPedidosAV.Domain.Entities;

namespace GestionPedidosAV.Domain.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
    Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    Task<bool> IsStockAvailableAsync(int productId, int quantity);
    Task UpdateStockAsync(int productId, int quantity);
    Task<IEnumerable<Product>> GetAvailableProductsAsync();
}
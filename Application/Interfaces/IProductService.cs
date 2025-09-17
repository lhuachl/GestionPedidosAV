using GestionPedidosAV.Application.DTOs;

namespace GestionPedidosAV.Application.Interfaces;

public interface IProductService
{
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<IEnumerable<ProductDto>> GetAvailableProductsAsync();
    Task<IEnumerable<ProductDto>> SearchProductsAsync(ProductSearchDto searchDto);
    Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
    Task<ProductDto> UpdateProductAsync(UpdateProductDto updateProductDto);
    Task DeleteProductAsync(int id);
    Task<bool> IsStockAvailableAsync(int productId, int quantity);
    Task UpdateStockAsync(int productId, int quantity);
}
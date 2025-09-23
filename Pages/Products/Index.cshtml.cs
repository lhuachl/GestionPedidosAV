using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GestionPedidosAV.Application.Interfaces;
using GestionPedidosAV.Application.DTOs;

namespace GestionPedidosAV.Pages.Products;

public class IndexModel : PageModel
{
    private readonly IProductService _productService;

    public IndexModel(IProductService productService)
    {
        _productService = productService;
    }

    public List<ProductDto> Products { get; set; } = new();
    
    [BindProperty(SupportsGet = true)]
    public ProductSearchDto SearchFilter { get; set; } = new();

    public async Task OnGetAsync()
    {
        try
        {
            if (HasActiveFilters(SearchFilter))
            {
                var filteredProducts = await _productService.SearchProductsAsync(SearchFilter);
                Products = filteredProducts.ToList();
            }
            else
            {
                var allProducts = await _productService.GetAllProductsAsync();
                Products = allProducts.ToList();
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al cargar productos: {ex.Message}";
            Products = new List<ProductDto>();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            await _productService.DeleteProductAsync(id);
            TempData["SuccessMessage"] = "Producto eliminado correctamente";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al eliminar producto: {ex.Message}";
        }

        return RedirectToPage();
    }

    private bool HasActiveFilters(ProductSearchDto filter)
    {
        return !string.IsNullOrEmpty(filter.SearchTerm) ||
               !string.IsNullOrEmpty(filter.Category) ||
               filter.MinPrice.HasValue ||
               filter.MaxPrice.HasValue ||
               filter.Status.HasValue;
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GestionPedidosAV.Application.Interfaces;
using GestionPedidosAV.Application.DTOs;

namespace GestionPedidosAV.Pages.Products;

public class CreateModel : PageModel
{
    private readonly IProductService _productService;

    public CreateModel(IProductService productService)
    {
        _productService = productService;
    }

    [BindProperty]
    public CreateProductDto Product { get; set; } = new();

    public void OnGet()
    {
        // Initialize with default values
        Product.WeightUnit = "kg";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var createdProduct = await _productService.CreateProductAsync(Product);
            TempData["SuccessMessage"] = $"Producto '{createdProduct.Name}' creado correctamente";
            
            return RedirectToPage("./Index");
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return Page();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al crear producto: {ex.Message}");
            return Page();
        }
    }
}
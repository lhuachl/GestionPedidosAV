using GestionPedidosAV.Domain.Enums;

namespace GestionPedidosAV.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? Category { get; set; }
    public string? SKU { get; set; }
    public string? ImageUrl { get; set; }
    public ProductStatus Status { get; set; }
    public decimal Weight { get; set; }
    public string WeightUnit { get; set; } = "kg";
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? Category { get; set; }
    public string? SKU { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Weight { get; set; }
    public string WeightUnit { get; set; } = "kg";
}

public class UpdateProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? Category { get; set; }
    public string? SKU { get; set; }
    public string? ImageUrl { get; set; }
    public ProductStatus Status { get; set; }
    public decimal Weight { get; set; }
    public string WeightUnit { get; set; } = "kg";
}

public class ProductSearchDto
{
    public string? SearchTerm { get; set; }
    public string? Category { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public ProductStatus? Status { get; set; }
}
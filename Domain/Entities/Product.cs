using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionPedidosAV.Domain.Enums;

namespace GestionPedidosAV.Domain.Entities;

public class Product : BaseEntity
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Price { get; set; }
    
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }
    
    [StringLength(100)]
    public string? Category { get; set; }
    
    [StringLength(50)]
    public string? SKU { get; set; }
    
    [StringLength(500)]
    public string? ImageUrl { get; set; }
    
    public ProductStatus Status { get; set; } = ProductStatus.Active;
    
    public decimal Weight { get; set; }
    
    [StringLength(10)]
    public string WeightUnit { get; set; } = "kg";
    
    // Navigation Properties
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    
    public bool IsAvailable => Status == ProductStatus.Active && Stock > 0 && !IsDeleted;
}
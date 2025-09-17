using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionPedidosAV.Domain.Entities;

public class OrderItem : BaseEntity
{
    [Required]
    public int OrderId { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    public int Quantity { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }
    
    [StringLength(500)]
    public string? Notes { get; set; }
    
    // Navigation Properties
    [ForeignKey(nameof(OrderId))]
    public virtual Order Order { get; set; } = null!;
    
    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; } = null!;
    
    public void CalculateSubTotal()
    {
        SubTotal = UnitPrice * Quantity;
    }
}
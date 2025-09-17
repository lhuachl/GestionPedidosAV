using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionPedidosAV.Domain.Enums;

namespace GestionPedidosAV.Domain.Entities;

public class Order : BaseEntity
{
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [StringLength(20)]
    public string OrderNumber { get; set; } = string.Empty;
    
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    
    public OrderStatus Status { get; set; } = OrderStatus.Pendiente;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Tax { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal ShippingCost { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }
    
    [StringLength(500)]
    public string? ShippingAddress { get; set; }
    
    [StringLength(1000)]
    public string? Notes { get; set; }
    
    public DateTime? ShippedDate { get; set; }
    
    public DateTime? DeliveredDate { get; set; }
    
    [StringLength(100)]
    public string? TrackingNumber { get; set; }
    
    // Navigation Properties
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
    
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    
    public void CalculateTotal()
    {
        SubTotal = OrderItems.Sum(item => item.SubTotal);
        Tax = SubTotal * 0.15m; // 15% tax
        Total = SubTotal + Tax + ShippingCost;
    }
}
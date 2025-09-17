using System.ComponentModel.DataAnnotations;
using GestionPedidosAV.Domain.Enums;

namespace GestionPedidosAV.Domain.Entities;

public class User : BaseEntity
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(255, MinimumLength = 6)]
    public string PasswordHash { get; set; } = string.Empty;
    
    public UserRole Role { get; set; } = UserRole.Cliente;
    
    [Phone]
    [StringLength(15)]
    public string? PhoneNumber { get; set; }
    
    [StringLength(500)]
    public string? Address { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // Navigation Properties
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    
    public string FullName => $"{FirstName} {LastName}";
}
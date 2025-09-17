using Microsoft.EntityFrameworkCore;
using GestionPedidosAV.Domain.Entities;
using GestionPedidosAV.Domain.Enums;

namespace GestionPedidosAV.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User entity configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Role).HasConversion<int>();
        });

        // Product entity configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SKU).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.SKU).HasMaxLength(50);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.WeightUnit).HasMaxLength(10);
            entity.Property(e => e.Status).HasConversion<int>();
        });

        // Order entity configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Tax).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ShippingCost).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ShippingAddress).HasMaxLength(500);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.TrackingNumber).HasMaxLength(100);
            entity.Property(e => e.Status).HasConversion<int>();

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Orders)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // OrderItem entity configuration
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(e => e.Order)
                  .WithMany(o => o.OrderItems)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                  .WithMany(p => p.OrderItems)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Users
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FirstName = "Admin",
                LastName = "Sistema",
                Email = "admin@sistema.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = 2,
                FirstName = "Juan",
                LastName = "Pérez",
                Email = "juan.perez@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Cliente123!"),
                Role = UserRole.Cliente,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );

        // Seed Products
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Laptop HP Pavilion",
                Description = "Laptop HP Pavilion 15.6 pulgadas, Intel Core i5, 8GB RAM, 256GB SSD",
                Price = 899.99m,
                Stock = 10,
                Category = "Electrónicos",
                SKU = "HP-PAV-001",
                Status = ProductStatus.Active,
                Weight = 2.1m,
                WeightUnit = "kg",
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = 2,
                Name = "Mouse Logitech MX Master 3",
                Description = "Mouse inalámbrico Logitech MX Master 3, ergonómico y preciso",
                Price = 99.99m,
                Stock = 25,
                Category = "Accesorios",
                SKU = "LOG-MX3-001",
                Status = ProductStatus.Active,
                Weight = 0.14m,
                WeightUnit = "kg",
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
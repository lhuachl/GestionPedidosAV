using FluentValidation;
using GestionPedidosAV.Application.DTOs;

namespace GestionPedidosAV.Application.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del producto es requerido")
            .Length(2, 200).WithMessage("El nombre debe tener entre 2 y 200 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("La descripción no puede exceder 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a 0")
            .LessThan(1000000).WithMessage("El precio no puede exceder $1,000,000");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo");

        RuleFor(x => x.Category)
            .MaximumLength(100).WithMessage("La categoría no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Category));

        RuleFor(x => x.SKU)
            .MaximumLength(50).WithMessage("El SKU no puede exceder 50 caracteres")
            .Matches("^[A-Z0-9-]+$").WithMessage("El SKU solo puede contener letras mayúsculas, números y guiones")
            .When(x => !string.IsNullOrEmpty(x.SKU));

        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("El peso debe ser mayor a 0")
            .When(x => x.Weight > 0);

        RuleFor(x => x.WeightUnit)
            .NotEmpty().WithMessage("La unidad de peso es requerida")
            .Must(x => new[] { "kg", "g", "lb", "oz" }.Contains(x.ToLower()))
            .WithMessage("La unidad de peso debe ser: kg, g, lb, oz");
    }
}
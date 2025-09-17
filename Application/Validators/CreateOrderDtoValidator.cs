using FluentValidation;
using GestionPedidosAV.Application.DTOs;

namespace GestionPedidosAV.Application.Validators;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("El usuario es requerido");

        RuleFor(x => x.ShippingAddress)
            .NotEmpty().WithMessage("La dirección de envío es requerida")
            .MaximumLength(500).WithMessage("La dirección no puede exceder 500 caracteres");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Las notas no pueden exceder 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Notes));

        RuleFor(x => x.ShippingCost)
            .GreaterThanOrEqualTo(0).WithMessage("El costo de envío no puede ser negativo");

        RuleFor(x => x.OrderItems)
            .NotEmpty().WithMessage("El pedido debe tener al menos un producto")
            .Must(items => items.Count <= 50).WithMessage("El pedido no puede tener más de 50 productos");

        RuleForEach(x => x.OrderItems).SetValidator(new CreateOrderItemDtoValidator());
    }
}

public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
{
    public CreateOrderItemDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("El producto es requerido");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0")
            .LessThanOrEqualTo(1000).WithMessage("La cantidad no puede exceder 1000 unidades");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Las notas no pueden exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
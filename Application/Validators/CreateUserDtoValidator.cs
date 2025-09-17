using FluentValidation;
using GestionPedidosAV.Application.DTOs;

namespace GestionPedidosAV.Application.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es requerido")
            .Length(2, 100).WithMessage("El nombre debe tener entre 2 y 100 caracteres")
            .Matches("^[a-zA-ZáéíóúÁÉÍÓÚñÑ\\s]+$").WithMessage("El nombre solo puede contener letras");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es requerido")
            .Length(2, 100).WithMessage("El apellido debe tener entre 2 y 100 caracteres")
            .Matches("^[a-zA-ZáéíóúÁÉÍÓÚñÑ\\s]+$").WithMessage("El apellido solo puede contener letras");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El formato del email no es válido")
            .MaximumLength(255).WithMessage("El email no puede exceder 255 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres")
            .MaximumLength(255).WithMessage("La contraseña no puede exceder 255 caracteres")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("La contraseña debe contener al menos una mayúscula, una minúscula y un número");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("El formato del teléfono no es válido")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("La dirección no puede exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Address));
    }
}
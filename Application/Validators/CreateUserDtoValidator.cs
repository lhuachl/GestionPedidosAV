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
            .Matches("^[a-zA-Z������������\\s]+$").WithMessage("El nombre solo puede contener letras");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es requerido")
            .Length(2, 100).WithMessage("El apellido debe tener entre 2 y 100 caracteres")
            .Matches("^[a-zA-Z������������\\s]+$").WithMessage("El apellido solo puede contener letras");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El formato del email no es v�lido")
            .MaximumLength(255).WithMessage("El email no puede exceder 255 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contrase�a es requerida")
            .MinimumLength(6).WithMessage("La contrase�a debe tener al menos 6 caracteres")
            .MaximumLength(255).WithMessage("La contrase�a no puede exceder 255 caracteres")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("La contrase�a debe contener al menos una may�scula, una min�scula y un n�mero");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("El formato del tel�fono no es v�lido")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("La direcci�n no puede exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Address));
    }
}
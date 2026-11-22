using FluentValidation;
using NexoMovil.Core.DTOs;
using NexoMovil.Core.Enums;

namespace NexoMovil.Services.Validators.User
{
    /// <summary>
    /// Validaciones de entrada para la creación de un usuario (CU-07).
    /// </summary>
    public class CrearUsuarioDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CrearUsuarioDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede superar 50 caracteres.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("El apellido es obligatorio.")
                .MaximumLength(50).WithMessage("El apellido no puede superar 50 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es obligatorio.")
                .EmailAddress().WithMessage("El email no tiene un formato válido.")
                .MaximumLength(100).WithMessage("El email no puede superar 100 caracteres.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.")
                .MaximumLength(100).WithMessage("La contraseña no puede superar 100 caracteres.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("El rol es obligatorio.")
                .Must(BeAValidRole)
                .WithMessage("El rol debe ser 'Admin' o 'Seller'.");

            RuleFor(x => x.Telephone)
                .MaximumLength(15).WithMessage("El teléfono no puede superar 15 caracteres.")
                .When(x => x.Telephone != null);
        }

        private static bool BeAValidRole(string role)
        {
            return role == nameof(RoleType.Admin) || role == nameof(RoleType.Seller);
        }
    }
}

using FluentValidation;
using NexoMovil.Core.DTOs;

namespace NexoMovil.Services.Validators.Product
{
    /// <summary>
    /// Validaciones de entrada para la actualización de un producto del catálogo.
    /// </summary>
    public class ActualizarProductoDtoValidator : AbstractValidator<ProductDto>
    {
        public ActualizarProductoDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El identificador del producto debe ser mayor que cero.");

            RuleFor(x => x.Model)
                .NotEmpty().WithMessage("El modelo es obligatorio.")
                .MaximumLength(100).WithMessage("El modelo no debe exceder los 100 caracteres.");

            RuleFor(x => x.Brand)
                .NotEmpty().WithMessage("La marca es obligatoria.")
                .MaximumLength(50).WithMessage("La marca no puede exceder los 50 caracteres.");

            // RN-09: El stock de un producto bajo ninguna circunstancia puede adquirir valores negativos.
            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo.");

            // RN-08: El precio unitario de registro de un producto debe ser mayor a cero.
            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.");
        }
    }
}

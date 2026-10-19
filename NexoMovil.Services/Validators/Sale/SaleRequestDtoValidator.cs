using FluentValidation;
using NexoMovil.Core.DTOs;

namespace NexoMovil.Services.Validators.Sale
{
    /// <summary>
    /// Validaciones de entrada del CU-02 (procesamiento de venta).
    /// Las comprobaciones de existencia, stock y estado del usuario se realizan
    /// en el controlador porque dependen de la base de datos.
    /// </summary>
    public class SaleRequestDtoValidator : AbstractValidator<SaleRequestDto>
    {
        public SaleRequestDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("El identificador del producto debe ser mayor que cero.");

            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("El identificador del usuario debe ser mayor que cero.");

            // RN-11: Toda venta debe registrar una cantidad igual o superior a uno.
            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("La cantidad de la venta debe ser igual o superior a 1.");
        }
    }
}

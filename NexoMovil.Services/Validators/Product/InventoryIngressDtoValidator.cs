using FluentValidation;
using NexoMovil.Core.DTOs;

namespace NexoMovil.Services.Validators.Product
{
    /// <summary>
    /// Validaciones de entrada del CU-01 (ingreso de inventario).
    /// </summary>
    public class InventoryIngressDtoValidator : AbstractValidator<InventoryIngressDto>
    {
        public InventoryIngressDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("El identificador del producto debe ser mayor que cero.");

            // RN-03: La cantidad ingresada al inventario debe ser estrictamente mayor que cero.
            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("La cantidad debe ser mayor a cero.");
        }
    }
}

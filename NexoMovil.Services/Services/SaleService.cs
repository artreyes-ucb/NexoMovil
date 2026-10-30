using NexoMovil.Core.Entities;
using NexoMovil.Core.Exceptions;
using NexoMovil.Core.Interfaces;
using NexoMovil.Services.Interfaces;
using System.Net;

namespace NexoMovil.Services.Services
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public SaleService(
            ISaleRepository saleRepository,
            IProductRepository productRepository,
            IUserRepository userRepository)
        {
            _saleRepository = saleRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<Sale>> GetAllSalesAsync()
        {
            return await _saleRepository.GetAllWithDetailsAsync();
        }

        public async Task<Sale?> GetSaleByIdAsync(int id)
        {
            return await _saleRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<Sale> InsertSaleAsync(Sale sale)
        {
            // RN-11: toda venta debe incluir una cantidad igual o mayor a 1.
            if (sale.Quantity <= 0)
                throw new BusinessException("La cantidad de la venta debe ser igual o superior a 1.", HttpStatusCode.BadRequest);

            // Validación relacional: el producto debe existir antes de registrar la venta.
            var product = await _productRepository.GetByIdAsync(sale.ProductId);
            if (product == null)
                throw new BusinessException("El producto no existe.", HttpStatusCode.NotFound);

            // Validación relacional: el usuario (vendedor) debe existir antes de registrar la venta.
            var user = await _userRepository.GetByIdAsync(sale.UserId);
            if (user == null)
                throw new BusinessException("El usuario no existe.", HttpStatusCode.NotFound);

            // Precondición de CU-02: el vendedor debe contar con permisos activos.
            if (!user.IsActive)
                throw new BusinessException("Usuario inactivo, contacte al administrador.", HttpStatusCode.Forbidden);

            // RN-08: el precio unitario debe ser mayor a cero.
            if (product.Price <= 0)
                throw new BusinessException("El producto no tiene un precio unitario válido.", HttpStatusCode.BadRequest);

            // RN-01: no se puede vender más de lo que hay en stock.
            // El informe (Figura 6) muestra 409 Conflict para este caso.
            if (product.Stock < sale.Quantity)
                throw new BusinessException($"Stock insuficiente. Disponible: {product.Stock}, solicitado: {sale.Quantity}.", HttpStatusCode.Conflict);

            sale.Date = DateTime.Now;
            sale.IsActive = true;

            // RN-02: el total se calcula en el servidor con el precio vigente del producto.
            sale.TotalAmount = Math.Round(product.Price * sale.Quantity, 2, MidpointRounding.AwayFromZero);

            // RN-09: el stock se descuenta según la cantidad vendida.
            product.Stock -= sale.Quantity;

            _productRepository.Update(product);
            await _saleRepository.Add(sale);
            await _saleRepository.SaveChangesAsync();

            // Se asignan después de persistir para que EF Core no intente insertar
            // entidades que ya existen; el mapeo de respuesta necesita estos datos.
            sale.Product = product;
            sale.User = user;

            return sale;
        }

        public async Task UpdateSaleAsync(Sale sale)
        {
            _saleRepository.Update(sale);
            await _saleRepository.SaveChangesAsync();
        }

        public async Task DeleteSaleAsync(int id)
        {
            var sale = await _saleRepository.GetByIdAsync(id);
            if (sale == null)
                throw new BusinessException("Venta no encontrada.", HttpStatusCode.NotFound);

            // RN-05: las ventas no se eliminan físicamente; se anulan con IsActive = false.
            if (!sale.IsActive)
                throw new BusinessException("La venta ya se encuentra anulada.", HttpStatusCode.Conflict);

            sale.IsActive = false;

            _saleRepository.Update(sale);
            await _saleRepository.SaveChangesAsync();
        }
    }
}

using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;
using NexoMovil.Core.Exceptions;
using NexoMovil.Core.Helpers;
using NexoMovil.Core.Interfaces;
using NexoMovil.Core.QueryFilters;
using NexoMovil.Services.Interfaces;
using System.Globalization;
using System.Net;

namespace NexoMovil.Services.Services
{
    public class SaleService : ISaleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SaleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SaleResponseDto>> GetAllSalesDapperAsync(int limit = 10)
        {
            return await _unitOfWork.SaleRepository.GetAllWithDetailsDapperAsync(limit);
        }

        public async Task<IEnumerable<SaleResponseDto>> GetSalesAsync(SaleQueryFilter filter)
        {
            if (filter.UserId.HasValue && filter.UserId.Value <= 0)
                throw new BusinessException("El identificador del vendedor debe ser mayor a cero.", HttpStatusCode.BadRequest);

            if (filter.ProductId.HasValue && filter.ProductId.Value <= 0)
                throw new BusinessException("El identificador del producto debe ser mayor a cero.", HttpStatusCode.BadRequest);

            return await _unitOfWork.SaleRepository.GetFilteredWithDetailsDapperAsync(filter);
        }

        public async Task<SaleResponseDto?> GetSaleByIdAsync(int id)
        {
            return await _unitOfWork.SaleRepository.GetByIdWithDetailsDapperAsync(id);
        }

        public async Task<Sale> InsertSaleAsync(Sale sale)
        {
            // RN-11: toda venta debe incluir una cantidad igual o mayor a 1.
            if (sale.Quantity <= 0)
                throw new BusinessException("La cantidad de la venta debe ser igual o superior a 1.", HttpStatusCode.BadRequest);

            // Validación relacional: el producto debe existir antes de registrar la venta.
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(sale.ProductId);
            if (product == null)
                throw new BusinessException("El producto no existe.", HttpStatusCode.NotFound);

            // Validación relacional: el usuario (vendedor) debe existir antes de registrar la venta.
            var user = await _unitOfWork.UserRepository.GetByIdAsync(sale.UserId);
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

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaleRepository.Add(sale);
            await _unitOfWork.SaveChangesAsync();

            // Se asignan después de persistir para que EF Core no intente insertar
            // entidades que ya existen; el mapeo de respuesta necesita estos datos.
            sale.Product = product;
            sale.User = user;

            return sale;
        }

        public async Task UpdateSaleAsync(Sale sale)
        {
            _unitOfWork.SaleRepository.Update(sale);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<CashCloseReportDto> GetDailyCashCloseReportAsync(string? date)
        {
            DateTime reportDate;

            if (string.IsNullOrWhiteSpace(date))
            {
                reportDate = DateTime.Today;
            }
            else
            {
                var parsedDate = Procesos.ParseFechaFlexible(date);
                if (parsedDate is null)
                    throw new BusinessException("Fecha no válida. Use el formato dd-MM-yyyy.", HttpStatusCode.BadRequest);

                reportDate = DateTime.ParseExact(parsedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            // RN-10: el reporte solo considera ventas activas.
            var sellers = (await _unitOfWork.SaleRepository.GetDailyReportAsync(reportDate)).ToList();

            return new CashCloseReportDto
            {
                Date = reportDate.ToString("yyyy-MM-dd"),
                TotalSales = sellers.Sum(s => s.SalesCount),
                TotalAmount = Math.Round(sellers.Sum(s => s.SubTotal), 2, MidpointRounding.AwayFromZero),
                Sellers = sellers
            };
        }

        public async Task AnnulSaleAsync(int id)
        {
            var sale = await _unitOfWork.SaleRepository.GetByIdAsync(id);
            if (sale == null)
                throw new BusinessException("Venta no encontrada.", HttpStatusCode.NotFound);

            // RN-05: las ventas no se eliminan físicamente; se anulan con IsActive = false.
            if (!sale.IsActive)
                throw new BusinessException("La venta ya se encuentra anulada.", HttpStatusCode.Conflict);

            sale.IsActive = false;

            _unitOfWork.SaleRepository.Update(sale);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}

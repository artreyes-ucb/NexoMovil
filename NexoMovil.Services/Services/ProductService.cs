using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;
using NexoMovil.Core.Exceptions;
using NexoMovil.Core.Interfaces;
using NexoMovil.Core.QueryFilters;
using NexoMovil.Services.Interfaces;
using System.Net;

namespace NexoMovil.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Product>> GetAvailableProductsAsync(int limit = 10)
        {
            return await _unitOfWork.ProductRepository.GetAllAvailableDapperAsync(limit);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(ProductQueryFilter filter)
        {
            if (filter.MaxPrice.HasValue && filter.MaxPrice.Value < 0)
                throw new BusinessException("El precio máximo no puede ser negativo.", HttpStatusCode.BadRequest);

            return await _unitOfWork.ProductRepository.GetFilteredDapperAsync(filter);
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _unitOfWork.ProductRepository.GetByIdDapperAsync(id);
        }

        public async Task InsertProductAsync(Product product)
        {
            // RN-08: el precio unitario debe ser mayor a cero.
            if (product.Price <= 0)
                throw new BusinessException("El precio unitario debe ser mayor a 0.", HttpStatusCode.BadRequest);

            // RN-09: el stock no puede ser negativo.
            if (product.Stock < 0)
                throw new BusinessException("El stock no puede ser negativo.", HttpStatusCode.BadRequest);

            product.CreatedAt = DateTime.Now;

            await _unitOfWork.ProductRepository.Add(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            // RN-08: el precio unitario debe ser mayor a cero.
            if (product.Price <= 0)
                throw new BusinessException("El precio unitario debe ser mayor a 0.", HttpStatusCode.BadRequest);

            // RN-09: el stock no puede ser negativo.
            if (product.Stock < 0)
                throw new BusinessException("El stock no puede ser negativo.", HttpStatusCode.BadRequest);

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null)
                throw new BusinessException("Producto no encontrado.", HttpStatusCode.NotFound);

            if (await _unitOfWork.SaleRepository.ExistsByProductIdAsync(id))
                throw new BusinessException("No se puede eliminar el producto porque tiene ventas asociadas.", HttpStatusCode.Conflict);

            await _unitOfWork.ProductRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Product> AddInventoryIngressAsync(InventoryIngressDto dto)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.ProductId);
            if (product == null)
                throw new BusinessException("Producto no encontrado.", HttpStatusCode.NotFound);

            // RN-03: la cantidad ingresada al inventario debe ser estrictamente mayor a cero.
            if (dto.Quantity <= 0)
                throw new BusinessException("La cantidad debe ser mayor a cero.", HttpStatusCode.BadRequest);

            // RN-03 y RN-09: la cantidad ya está validada (> 0) y el stock solo crece.
            product.Stock += dto.Quantity;

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return product;
        }
    }
}

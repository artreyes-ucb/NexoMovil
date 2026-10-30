using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;
using NexoMovil.Core.Exceptions;
using NexoMovil.Core.Interfaces;
using NexoMovil.Services.Interfaces;
using System.Net;

namespace NexoMovil.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ISaleRepository _saleRepository;

        public ProductService(
            IProductRepository productRepository,
            ISaleRepository saleRepository)
        {
            _productRepository = productRepository;
            _saleRepository = saleRepository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Product>> GetAvailableProductsAsync()
        {
            return await _productRepository.GetAllAvailableAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
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

            await _productRepository.Add(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            // RN-08: el precio unitario debe ser mayor a cero.
            if (product.Price <= 0)
                throw new BusinessException("El precio unitario debe ser mayor a 0.", HttpStatusCode.BadRequest);

            // RN-09: el stock no puede ser negativo.
            if (product.Stock < 0)
                throw new BusinessException("El stock no puede ser negativo.", HttpStatusCode.BadRequest);

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new BusinessException("Producto no encontrado.", HttpStatusCode.NotFound);

            if (await _saleRepository.ExistsByProductIdAsync(id))
                throw new BusinessException("No se puede eliminar el producto porque tiene ventas asociadas.", HttpStatusCode.Conflict);

            await _productRepository.Delete(id);
            await _productRepository.SaveChangesAsync();
        }

        public async Task<Product> AddInventoryIngressAsync(InventoryIngressDto dto)
        {
            var product = await _productRepository.GetByIdAsync(dto.ProductId);
            if (product == null)
                throw new BusinessException("Producto no encontrado.", HttpStatusCode.NotFound);

            // RN-03: la cantidad ingresada al inventario debe ser estrictamente mayor a cero.
            if (dto.Quantity <= 0)
                throw new BusinessException("La cantidad debe ser mayor a cero.", HttpStatusCode.BadRequest);

            // RN-03 y RN-09: la cantidad ya está validada (> 0) y el stock solo crece.
            product.Stock += dto.Quantity;

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();

            return product;
        }
    }
}

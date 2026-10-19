using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NexoMovil.Api.Response;
using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;
using NexoMovil.Core.Interfaces;
using NexoMovil.Services.Validators.Product;

namespace NexoMovil.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;
        private readonly CrearProductoDtoValidator _crearProductoValidator;
        private readonly ActualizarProductoDtoValidator _actualizarProductoValidator;
        private readonly IValidator<InventoryIngressDto> _inventoryValidator;

        public ProductController(
            IProductRepository productRepository,
            ISaleRepository saleRepository,
            IMapper mapper,
            CrearProductoDtoValidator crearProductoValidator,
            ActualizarProductoDtoValidator actualizarProductoValidator,
            IValidator<InventoryIngressDto> inventoryValidator)
        {
            _productRepository = productRepository;
            _saleRepository = saleRepository;
            _mapper = mapper;
            _crearProductoValidator = crearProductoValidator;
            _actualizarProductoValidator = actualizarProductoValidator;
            _inventoryValidator = inventoryValidator;
        }

        // GET: api/product
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var products = await _productRepository.GetAllAvailableAsync();
                var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

                var response = new ApiResponse<IEnumerable<ProductDto>>(productDtos);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los productos", error = ex.Message });
            }
        }

        // GET: api/product/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await _productRepository.GetById(id);
                if (product == null)
                    return NotFound(new { message = "Producto no encontrado." });

                var response = new ApiResponse<ProductDto>(_mapper.Map<ProductDto>(product));
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el producto", error = ex.Message });
            }
        }

        // POST: api/product
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] ProductDto newProductDto)
        {
            try
            {
                await _crearProductoValidator.ValidateAndThrowAsync(newProductDto);

                var product = _mapper.Map<Product>(newProductDto);
                product.CreatedAt = DateTime.Now;

                await _productRepository.Add(product);
                await _productRepository.SaveChangesAsync();

                var createdDto = _mapper.Map<ProductDto>(product);

                var response = new ApiResponse<ProductDto>(createdDto);
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = "Datos de entrada inválidos.", errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el producto", error = ex.Message });
            }
        }

        // PUT: api/product/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDto productDto)
        {
            try
            {
                if (id != productDto.Id)
                    return BadRequest(new { message = "El id del producto no coincide." });

                await _actualizarProductoValidator.ValidateAndThrowAsync(productDto);

                var product = await _productRepository.GetById(id);
                if (product == null)
                    return NotFound(new { message = "Producto no encontrado." });

                _mapper.Map(productDto, product);

                _productRepository.Update(product);
                await _productRepository.SaveChangesAsync();

                var response = new ApiResponse<ProductDto>(_mapper.Map<ProductDto>(product));
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = "Datos de entrada inválidos.", errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el producto", error = ex.Message });
            }
        }

        // DELETE: api/product/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await _productRepository.GetById(id);
                if (product == null)
                    return NotFound(new { message = "Producto no encontrado." });

        
                if (await _saleRepository.ExistsByProductIdAsync(id))
                    return Conflict(new { message = "No se puede eliminar el producto porque tiene ventas asociadas." });

                await _productRepository.Delete(id);
                await _productRepository.SaveChangesAsync();

                var response = new ApiResponse<string>("Producto eliminado correctamente.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el producto", error = ex.Message });
            }
        }

        // POST: api/product/ingreso-inventario (CU-01)
        [HttpPost("ingreso-inventario")]
        public async Task<IActionResult> IngresarInventario([FromBody] InventoryIngressDto dto)
        {
            try
            {
                await _inventoryValidator.ValidateAndThrowAsync(dto);

                var product = await _productRepository.GetById(dto.ProductId);
                if (product == null)
                    return NotFound(new { message = "Producto no encontrado." });

                // RN-03 y RN-09: la cantidad ya está validada (> 0) y el stock solo crece.
                product.Stock += dto.Quantity;

                _productRepository.Update(product);
                await _productRepository.SaveChangesAsync();

                var response = new ApiResponse<ProductDto>(_mapper.Map<ProductDto>(product));
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = "Datos de entrada inválidos.", errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al ingresar el inventario", error = ex.Message });
            }
        }
    }
}

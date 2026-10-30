using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NexoMovil.Api.Response;
using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;
using NexoMovil.Core.Exceptions;
using NexoMovil.Services.Interfaces;
using NexoMovil.Services.Validators.Product;
using System.Net;

namespace NexoMovil.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly CrearProductoDtoValidator _crearProductoValidator;
        private readonly ActualizarProductoDtoValidator _actualizarProductoValidator;
        private readonly IValidator<InventoryIngressDto> _inventoryValidator;

        public ProductController(
            IProductService productService,
            IMapper mapper,
            CrearProductoDtoValidator crearProductoValidator,
            ActualizarProductoDtoValidator actualizarProductoValidator,
            IValidator<InventoryIngressDto> inventoryValidator)
        {
            _productService = productService;
            _mapper = mapper;
            _crearProductoValidator = crearProductoValidator;
            _actualizarProductoValidator = actualizarProductoValidator;
            _inventoryValidator = inventoryValidator;
        }

        // CU-03: Consultar Stock. Se mantiene el listado de productos con stock.
        // GET: api/product
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAvailableProductsAsync();
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            var response = new ApiResponse<IEnumerable<ProductDto>>(productDtos);
            return Ok(response);
        }

        // CU-03: detalle de un producto por su Id.
        // GET: api/product/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                throw new BusinessException("Producto no encontrado.", HttpStatusCode.NotFound);

            var response = new ApiResponse<ProductDto>(_mapper.Map<ProductDto>(product));
            return Ok(response);
        }

        // CU-03: listado de productos con un límite opcional.
        // Devuelve como máximo la cantidad indicada en limit.
        [HttpGet("dapper")]
        public async Task<IActionResult> GetProductsWithLimit([FromQuery] int limit = 10)
        {
            if (limit <= 0)
                throw new BusinessException("El límite debe ser mayor a cero.", HttpStatusCode.BadRequest);

            var products = await _productService.GetAvailableProductsAsync();
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products.Take(limit));

            return Ok(new ApiResponse<IEnumerable<ProductDto>>(productDtos));
        }

        // POST: api/product
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] ProductDto newProductDto)
        {
            var validation = await _crearProductoValidator.ValidateAsync(newProductDto);
            if (!validation.IsValid)
            {
                throw new BusinessException(
                    message: "Errores de validación en los datos enviados.",
                    details: validation.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        error = e.ErrorMessage
                    }));
            }

            try
            {
                var product = _mapper.Map<Product>(newProductDto);

                await _productService.InsertProductAsync(product);

                var createdDto = _mapper.Map<ProductDto>(product);

                var response = new ApiResponse<ProductDto>(createdDto);
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, response);
            }
            catch (BusinessException)
            {
                // Se re-lanza para que el middleware genere la respuesta estructurada.
                throw;
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
            if (id != productDto.Id)
                throw new BusinessException("El id del producto no coincide.", HttpStatusCode.BadRequest);

            var validation = await _actualizarProductoValidator.ValidateAsync(productDto);
            if (!validation.IsValid)
            {
                throw new BusinessException(
                    message: "Errores de validación en los datos enviados.",
                    details: validation.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        error = e.ErrorMessage
                    }));
            }

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                throw new BusinessException("Producto no encontrado.", HttpStatusCode.NotFound);

            try
            {
                _mapper.Map(productDto, product);

                await _productService.UpdateProductAsync(product);

                var response = new ApiResponse<ProductDto>(_mapper.Map<ProductDto>(product));
                return Ok(response);
            }
            catch (BusinessException)
            {
                // Se re-lanza para que el middleware genere la respuesta estructurada.
                throw;
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
            if (id <= 0)
                throw new BusinessException("El ID debe ser mayor que cero.", HttpStatusCode.BadRequest);

            try
            {
                await _productService.DeleteProductAsync(id);

                var response = new ApiResponse<string>("Producto eliminado correctamente.");
                return Ok(response);
            }
            catch (BusinessException)
            {
                // Se re-lanza para que el middleware genere la respuesta estructurada.
                throw;
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
            var validation = await _inventoryValidator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                throw new BusinessException(
                    message: "Errores de validación en los datos enviados.",
                    details: validation.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        error = e.ErrorMessage
                    }));
            }

            try
            {
                var product = await _productService.AddInventoryIngressAsync(dto);

                var response = new ApiResponse<ProductDto>(_mapper.Map<ProductDto>(product));
                return Ok(response);
            }
            catch (BusinessException)
            {
                // Se re-lanza para que el middleware genere la respuesta estructurada.
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al registrar el ingreso de inventario", error = ex.Message });
            }
        }
    }
}

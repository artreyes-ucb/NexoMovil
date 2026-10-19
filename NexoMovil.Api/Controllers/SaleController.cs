using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NexoMovil.Api.Response;
using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;
using NexoMovil.Core.Interfaces;
using NexoMovil.Services.Validators.Sale;

namespace NexoMovil.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IProductRepository _productRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<SaleRequestDto> _saleValidator;

        public SaleController(
            ISaleRepository saleRepository,
            IProductRepository productRepository,
            IBaseRepository<User> userRepository,
            IMapper mapper,
            IValidator<SaleRequestDto> saleValidator)
        {
            _saleRepository = saleRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _saleValidator = saleValidator;
        }

        // GET: api/sale (CU-02: listar ventas)
        [HttpGet]
        public async Task<IActionResult> GetSales()
        {
            try
            {
                var sales = await _saleRepository.GetAllWithDetailsAsync();
                var saleDtos = _mapper.Map<IEnumerable<SaleResponseDto>>(sales);

                var response = new ApiResponse<IEnumerable<SaleResponseDto>>(saleDtos);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las ventas", error = ex.Message });
            }
        }

        // GET: api/sale/dapper?limit=5 (legacy: listar ventas con límite)
        [HttpGet("dapper")]
        public async Task<IActionResult> GetSalesWithLimit([FromQuery] int limit = 5)
        {
            try
            {
                if (limit <= 0)
                    return BadRequest(new { message = "El límite debe ser mayor a cero." });

                var sales = await _saleRepository.GetAllWithDetailsAsync();
                var saleDtos = _mapper.Map<IEnumerable<SaleResponseDto>>(sales.Take(limit));

                var response = new ApiResponse<IEnumerable<SaleResponseDto>>(saleDtos);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las ventas", error = ex.Message });
            }
        }

        // GET: api/sale/1 (CU-02: obtener el detalle de una venta)
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSaleById(int id)
        {
            try
            {
                var sale = await _saleRepository.GetByIdWithDetailsAsync(id);
                if (sale == null)
                    return NotFound(new { message = "Venta no encontrada." });

                var response = new ApiResponse<SaleResponseDto>(_mapper.Map<SaleResponseDto>(sale));
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener la venta", error = ex.Message });
            }
        }

        // DELETE: api/sale/1 (baja lógica, RN-05)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            try
            {
                var sale = await _saleRepository.GetById(id);
                if (sale == null)
                    return NotFound(new { message = "Venta no encontrada." });

                // RN-05: las ventas no se eliminan físicamente; se anulan con IsActive = false.
                if (!sale.IsActive)
                    return Conflict(new { message = "La venta ya se encuentra anulada." });

                sale.IsActive = false;

                _saleRepository.Update(sale);
                await _saleRepository.SaveChangesAsync();

                var response = new ApiResponse<string>("Venta anulada correctamente.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al anular la venta", error = ex.Message });
            }
        }

        // POST: api/sale/procesar (CU-02: procesar una nueva venta)
        [HttpPost("procesar")]
        public async Task<IActionResult> ProcesarVenta([FromBody] SaleRequestDto dto)
        {
            try
            {
                // Identificadores > 0 y cantidad >= 1 (RN-11) mediante FluentValidation.
                await _saleValidator.ValidateAndThrowAsync(dto);

                // Precondición de CU-02: el vendedor debe contar con permisos activos.
                var user = await _userRepository.GetById(dto.UserId);
                if (user == null)
                    return NotFound(new { message = "Usuario no encontrado." });

                if (!user.IsActive)
                    return StatusCode(403, new { message = "El usuario no está activo y no puede procesar ventas." });

                var product = await _productRepository.GetById(dto.ProductId);
                if (product == null)
                    return NotFound(new { message = "Producto no encontrado." });

                // RN-08: el precio unitario debe ser mayor a cero.
                if (product.Price <= 0)
                    return Conflict(new { message = "El producto no tiene un precio unitario válido." });

                // RN-01: no se puede vender más de lo que hay en stock.
                if (product.Stock < dto.Quantity)
                    return Conflict(new { message = $"Stock insuficiente. Disponible: {product.Stock}, solicitado: {dto.Quantity}." });

                // RN-02: el total se calcula en el servidor con el precio vigente del producto.
                var totalAmount = Math.Round(product.Price * dto.Quantity, 2, MidpointRounding.AwayFromZero);

                var sale = _mapper.Map<Sale>(dto);
                sale.TotalAmount = totalAmount;

                // RN-09: el stock se descuenta según la cantidad vendida.
                product.Stock -= dto.Quantity;

                _productRepository.Update(product);
                await _saleRepository.Add(sale);
                await _saleRepository.SaveChangesAsync();

                // Se asignan después de persistir para que EF Core no intente insertar
                // entidades que ya existen; el mapeo de respuesta necesita estos datos.
                sale.Product = product;
                sale.User = user;

                var response = new ApiResponse<SaleResponseDto>(_mapper.Map<SaleResponseDto>(sale));
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = "Datos de entrada inválidos.", errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al procesar la venta", error = ex.Message });
            }
        }
    }
}

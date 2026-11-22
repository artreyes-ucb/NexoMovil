using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexoMovil.Api.Response;
using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;
using NexoMovil.Core.Enums;
using NexoMovil.Core.Exceptions;
using NexoMovil.Core.QueryFilters;
using NexoMovil.Services.Interfaces;
using NexoMovil.Services.Validators.Sale;
using System.Net;

namespace NexoMovil.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _saleService;
        private readonly IMapper _mapper;
        private readonly IValidator<SaleRequestDto> _saleValidator;

        public SaleController(
            ISaleService saleService,
            IMapper mapper,
            IValidator<SaleRequestDto> saleValidator)
        {
            _saleService = saleService;
            _mapper = mapper;
            _saleValidator = saleValidator;
        }

        // GET: api/sale (CU-02: listar ventas)
        [HttpGet]
        public async Task<IActionResult> GetSales([FromQuery] SaleQueryFilter filter)
        {
            var saleDtos = await _saleService.GetSalesAsync(filter);

            var response = new ApiResponse<IEnumerable<SaleResponseDto>>(saleDtos);
            return Ok(response);
        }

        // CU-02: listado de ventas con un límite opcional.
        // Devuelve como máximo la cantidad indicada en limit.
        [HttpGet("dapper")]
        public async Task<IActionResult> GetSalesWithLimit([FromQuery] int limit = 5)
        {
            if (limit <= 0)
                throw new BusinessException("El límite debe ser mayor a cero.", HttpStatusCode.BadRequest);

            var saleDtos = await _saleService.GetAllSalesDapperAsync(limit);

            var response = new ApiResponse<IEnumerable<SaleResponseDto>>(saleDtos);
            return Ok(response);
        }

        // CU-05: reporte de cierre de caja diario. Solo Administrador.
        // GET: api/sale/reporte/cierre-diario?date=01-11-2026
        [Authorize(Roles = nameof(RoleType.Admin))]
        [HttpGet("reporte/cierre-diario")]
        public async Task<IActionResult> GetDailyCashCloseReport([FromQuery] string? date = null)
        {
            var report = await _saleService.GetDailyCashCloseReportAsync(date);

            var response = new ApiResponse<CashCloseReportDto>(report);
            return Ok(response);
        }

        // GET: api/sale/1 (CU-02: obtener el detalle de una venta)
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSaleById(int id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);
            if (sale == null)
                throw new BusinessException("Venta no encontrada.", HttpStatusCode.NotFound);

            var response = new ApiResponse<SaleResponseDto>(sale);
            return Ok(response);
        }

        // DELETE: api/sale/1 (CU-06: baja lógica, RN-05). Solo Administrador.
        [Authorize(Roles = nameof(RoleType.Admin))]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> AnnulSale(int id)
        {
            if (id <= 0)
                throw new BusinessException("El ID debe ser mayor que cero.", HttpStatusCode.BadRequest);

            try
            {
                await _saleService.AnnulSaleAsync(id);

                var response = new ApiResponse<string>("Venta anulada correctamente.");
                return Ok(response);
            }
            catch (BusinessException)
            {
                // Se re-lanza para que el middleware genere la respuesta estructurada.
                throw;
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
            // Identificadores > 0 y cantidad >= 1 (RN-11) mediante FluentValidation.
            var validation = await _saleValidator.ValidateAsync(dto);
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
                var sale = _mapper.Map<Sale>(dto);

                // El servicio valida la existencia del producto y del usuario
                // antes de insertar la venta (validación relacional).
                sale = await _saleService.InsertSaleAsync(sale);

                var response = new ApiResponse<SaleResponseDto>(_mapper.Map<SaleResponseDto>(sale));
                return Ok(response);
            }
            catch (BusinessException)
            {
                // Se re-lanza para que el middleware genere la respuesta estructurada
                // con el status code que definió el servicio (400, 403, 404 o 409).
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al procesar la venta", error = ex.Message });
            }
        }
    }
}

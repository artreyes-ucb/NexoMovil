using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NexoMovil.Api.Response;
using NexoMovil.Core.DTOs;
using NexoMovil.Core.Exceptions;
using NexoMovil.Services.Interfaces;
using System.Net;

namespace NexoMovil.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IMapper _mapper;

        public InvoiceController(
            IInvoiceService invoiceService,
            IMapper mapper)
        {
            _invoiceService = invoiceService;
            _mapper = mapper;
        }

        // CU-04: listar todas las facturas.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            var invoiceDtos = _mapper.Map<IEnumerable<InvoiceDto>>(invoices);

            return Ok(new ApiResponse<IEnumerable<InvoiceDto>>(invoiceDtos));
        }

        // CU-04: obtener una factura por su Id.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                throw new BusinessException("El identificador de la factura debe ser mayor a cero.", HttpStatusCode.BadRequest);

            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
                throw new BusinessException("Factura no encontrada.", HttpStatusCode.NotFound);

            return Ok(new ApiResponse<InvoiceDto>(_mapper.Map<InvoiceDto>(invoice)));
        }

        // CU-04: obtener la factura asociada a una venta.
        [HttpGet("sale/{saleId:int}")]
        public async Task<IActionResult> GetBySaleId(int saleId)
        {
            if (saleId <= 0)
                throw new BusinessException("El identificador de la venta debe ser mayor a cero.", HttpStatusCode.BadRequest);

            var invoice = await _invoiceService.GetInvoiceBySaleIdAsync(saleId);
            if (invoice == null)
                throw new BusinessException("No se encontró una factura para la venta indicada.", HttpStatusCode.NotFound);

            return Ok(new ApiResponse<InvoiceDto>(_mapper.Map<InvoiceDto>(invoice)));
        }

        // CU-04: generar la factura de una venta.
        [HttpPost("generate/{saleId:int}")]
        public async Task<IActionResult> Generate(int saleId)
        {
            if (saleId <= 0)
                throw new BusinessException("El identificador de la venta debe ser mayor a cero.", HttpStatusCode.BadRequest);

            try
            {
                var invoice = await _invoiceService.GenerateInvoiceAsync(saleId);
                var invoiceDto = _mapper.Map<InvoiceDto>(invoice);

                return Ok(new ApiResponse<InvoiceDto>(invoiceDto));
            }
            catch (BusinessException)
            {
                // Se re-lanza para que el middleware genere la respuesta estructurada.
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al generar la factura", error = ex.Message });
            }
        }
    }
}

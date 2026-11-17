using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NexoMovil.Api.Filters;
using NexoMovil.Core.DTOs;
using NexoMovil.Core.Interfaces;
using NexoMovil.Infrastructure.Data;
using NexoMovil.Infrastructure.Mappings;
using NexoMovil.Infrastructure.Repositories;
using NexoMovil.Services.Interfaces;
using NexoMovil.Services.Services;
using NexoMovil.Services.Validators.Product;
using NexoMovil.Services.Validators.Sale;

namespace NexoMovil.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configurar la BD MySql
            var connectionString = builder.Configuration.GetConnectionString("NexoMovilConnection");
            builder.Services.AddDbContext<NexoMovilContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            #endregion

            // Infraestructura Dapper
            builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            builder.Services.AddScoped<IDapperContext, DapperContext>();

            // Inyección de dependencias (Servicios)
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ISaleService, SaleService>();
            builder.Services.AddScoped<IInvoiceService, InvoiceService>();

            // Validadores de FluentValidation
            builder.Services.AddScoped<IValidator<InventoryIngressDto>, InventoryIngressDtoValidator>();
            builder.Services.AddScoped<IValidator<SaleRequestDto>, SaleRequestDtoValidator>();
            builder.Services.AddScoped<CrearProductoDtoValidator>();
            builder.Services.AddScoped<ActualizarProductoDtoValidator>();

            // Controllers & OpenAPI
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            // AutoMapper (Escanea todo el ensamblado de Infrastructure donde están los Mappings)
            builder.Services.AddAutoMapper(typeof(ProductProfile).Assembly);

            var app = builder.Build();

            // Manejo centralizado de excepciones de negocio y de validación.
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
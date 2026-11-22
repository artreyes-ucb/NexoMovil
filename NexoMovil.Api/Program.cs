using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NexoMovil.Api.Filters;
using NexoMovil.Core.CustomEntities;
using NexoMovil.Core.DTOs;
using NexoMovil.Core.Interfaces;
using NexoMovil.Infrastructure.Data;
using NexoMovil.Infrastructure.Mappings;
using NexoMovil.Infrastructure.Repositories;
using NexoMovil.Services.Interfaces;
using NexoMovil.Services.Services;
using NexoMovil.Services.Validators.Product;
using NexoMovil.Services.Validators.Sale;
using NexoMovil.Services.Validators.User;
using System.Text;

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
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ISecurityRepository, SecurityRepository>();
            builder.Services.AddScoped<ISecurityService, SecurityService>();
            builder.Services.AddScoped<IPasswordService, PasswordService>();

            // Validadores de FluentValidation
            builder.Services.AddScoped<IValidator<InventoryIngressDto>, InventoryIngressDtoValidator>();
            builder.Services.AddScoped<IValidator<SaleRequestDto>, SaleRequestDtoValidator>();
            builder.Services.AddScoped<CrearProductoDtoValidator>();
            builder.Services.AddScoped<ActualizarProductoDtoValidator>();
            builder.Services.AddScoped<CrearUsuarioDtoValidator>();
            builder.Services.AddScoped<ActualizarUsuarioDtoValidator>();

            // Configuración de PBKDF2 (tamaño de salt, tamaño de clave e iteraciones)
            builder.Services.Configure<PasswordOptions>(
                builder.Configuration.GetSection("PasswordOptions"));

            // Controllers & OpenAPI
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            // AutoMapper (Escanea todo el ensamblado de Infrastructure donde están los Mappings)
            builder.Services.AddAutoMapper(typeof(ProductProfile).Assembly);

            #region Autenticación JWT (CU-08)
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Authentication:Issuer"],
                    ValidAudience = builder.Configuration["Authentication:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretKey"]!))
                };
            });

            builder.Services.AddAuthorization();
            #endregion

            var app = builder.Build();

            // Manejo centralizado de excepciones de negocio y de validación.
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            // El orden importa: primero identificar al usuario, luego autorizar.
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

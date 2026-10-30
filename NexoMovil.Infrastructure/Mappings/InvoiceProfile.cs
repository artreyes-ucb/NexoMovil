using AutoMapper;
using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;

namespace NexoMovil.Infrastructure.Mappings
{
    public class InvoiceProfile : Profile
    {
        public InvoiceProfile()
        {
            // CU-04: proyección de la factura con venta, producto y vendedor.
            CreateMap<Invoice, InvoiceDto>()
                .ForMember(dest => dest.InvoiceId,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IssuedAt,
                    opt => opt.ConvertUsing<DateTimeToStringConverter, DateTime>(src => src.IssuedAt))
                .ForMember(dest => dest.SaleId,
                    opt => opt.MapFrom(src => src.SaleId))
                .ForMember(dest => dest.SaleDate,
                    opt => opt.ConvertUsing<DateTimeToStringConverter, DateTime>(src => src.Sale.Date))
                .ForMember(dest => dest.ProductId,
                    opt => opt.MapFrom(src => src.Sale.ProductId))
                .ForMember(dest => dest.Brand,
                    opt => opt.MapFrom(src => src.Sale.Product.Brand))
                .ForMember(dest => dest.Model,
                    opt => opt.MapFrom(src => src.Sale.Product.Model))
                .ForMember(dest => dest.UnitPrice,
                    opt => opt.MapFrom(src => src.Sale.Product.Price))
                .ForMember(dest => dest.Quantity,
                    opt => opt.MapFrom(src => src.Sale.Quantity))
                .ForMember(dest => dest.TotalAmount,
                    opt => opt.MapFrom(src => src.Sale.TotalAmount))
                .ForMember(dest => dest.UserId,
                    opt => opt.MapFrom(src => src.Sale.UserId))
                .ForMember(dest => dest.SellerName,
                    opt => opt.MapFrom(src => $"{src.Sale.User.FirstName} {src.Sale.User.LastName}"))
                .ForMember(dest => dest.SellerEmail,
                    opt => opt.MapFrom(src => src.Sale.User.Email));
        }
    }
}

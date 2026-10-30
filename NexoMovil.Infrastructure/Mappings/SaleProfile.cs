using AutoMapper;
using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;

namespace NexoMovil.Infrastructure.Mappings
{
    public class SaleProfile : Profile
    {
        public SaleProfile()
        {
            // CU-02: conversión del request de venta a la entidad Sale.
            // La fecha y el estado se asignan aquí; el total lo calcula SaleService (RN-02).
            CreateMap<SaleRequestDto, Sale>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<Sale, SaleResponseDto>()
                .ForMember(dest => dest.SaleId,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Date,
                    opt => opt.ConvertUsing<DateTimeToStringConverter, DateTime>(src => src.Date))
                .ForMember(dest => dest.Brand,
                    opt => opt.MapFrom(src => src.Product.Brand))
                .ForMember(dest => dest.Model,
                    opt => opt.MapFrom(src => src.Product.Model))
                .ForMember(dest => dest.UnitPrice,
                    opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.CustomerName,
                    opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.CustomerEmail,
                    opt => opt.MapFrom(src => src.User.Email));
        }
    }
}
using AutoMapper;
using NexoMovil.Core.DTOs;
using NexoMovil.Core.Entities;
using System.Globalization;

namespace NexoMovil.Infrastructure.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.RegistrationDate,
                    opt => opt.ConvertUsing<DateTimeToStringConverter, DateTime>(src => src.CreatedAt));

            CreateMap<ProductDto, Product>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }

    public class DateTimeToStringConverter : IValueConverter<DateTime, string>
    {
        public string Convert(DateTime source, ResolutionContext context)
        {
            return source.ToString("dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }

    public class StringToDateTimeConverter : IValueConverter<string, DateTime>
    {
        public DateTime Convert(string source, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentException("La fecha no puede estar vacía");

            source = source.Trim();

            source = source.Replace("a. m.", "AM")
                          .Replace("p. m.", "PM")
                          .Replace("a.m.", "AM")
                          .Replace("p.m.", "PM")
                          .Replace("am", "AM")
                          .Replace("pm", "PM");

            string[] formats = new[]
            {
                "dd-MM-yyyy",
                "dd-MM-yyyy HH:mm:ss",
                "dd-MM-yyyy hh:mm:ss tt",
                "dd-MM-yyyy H:mm:ss",
                "dd-MM-yyyy h:mm:ss tt",
                "dd/MM/yyyy",
                "dd/MM/yyyy HH:mm:ss",
                "dd/MM/yyyy hh:mm:ss tt",
                "yyyy-MM-dd",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd hh:mm:ss tt"
            };

            if (DateTime.TryParseExact(source, formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime result))
            {
                return result;
            }

            if (DateTime.TryParse(source, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }

            throw new FormatException($"No se pudo convertir la fecha '{source}' a DateTime. Formatos soportados: fecha, fecha y hora, fecha con AM/PM");
        }
    }
}
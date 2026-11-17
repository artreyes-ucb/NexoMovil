using System.ComponentModel;

namespace NexoMovil.Core.QueryFilters
{
    public class ProductQueryFilter
    {
        [Description("Filtrar por marca del producto.")]
        public string? Brand { get; set; }

        [Description("Filtrar por modelo del producto.")]
        public string? Model { get; set; }

        [Description("Precio máximo para filtrar.")]
        public decimal? MaxPrice { get; set; }

        [Description("Si es true, solo retorna productos con stock mayor a cero.")]
        public bool? OnlyAvailable { get; set; }
    }
}

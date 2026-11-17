using System.ComponentModel;

namespace NexoMovil.Core.QueryFilters
{
    public class SaleQueryFilter
    {
        [Description("ID del vendedor")]
        public int? UserId { get; set; }

        [Description("ID del producto")]
        public int? ProductId { get; set; }

        [Description("Filtra por estado de la venta: true=activas, false=anuladas, null=todas")]
        public bool? IsActive { get; set; }
    }
}

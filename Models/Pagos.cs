using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{
    public class Pagos
    {
        [Key]
        public int IdPago { get; set; }
        [Required]
        public int IdFactura { get; set; }
        [ForeignKey("IdFactura")]
        public Facturas Factura { get; set; }
        [Required]
        public DateTime FechaPago { get; set; }
        [Required]
        public decimal MontoPagado { get; set; }
        [Required]
        public string MetodoPago { get; set; }
    }
}

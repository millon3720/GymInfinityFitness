using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{
    public class Pagos
    {
        [Key]
        public int IdPago { get; set; }

        [Required(ErrorMessage = "El ID de la factura es obligatorio.")]
        public int IdFactura { get; set; }

        [ForeignKey("IdFactura")]
        public Facturas? Factura { get; set; }

        [Required(ErrorMessage = "La fecha de pago es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaPago { get; set; }

        [Required(ErrorMessage = "El monto pagado es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto pagado debe ser mayor a 0.")]
        public decimal MontoPagado { get; set; }

        [Required(ErrorMessage = "El método de pago es obligatorio.")]
        [StringLength(50, ErrorMessage = "El método de pago no puede exceder los 50 caracteres.")]
        public string MetodoPago { get; set; }
    }
}

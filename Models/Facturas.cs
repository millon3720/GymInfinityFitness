using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{
    public class Facturas
    {
        [Key]
        public int IdFactura { get; set; }

        [Required(ErrorMessage = "El cliente es obligatorio.")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "El usuario que registra la factura es obligatorio.")]
        public int IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuarios Usuario { get; set; }

        [Required(ErrorMessage = "La fecha de la factura es obligatoria.")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El total de la factura es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El total debe ser un valor positivo.")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "El estado de la factura es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El estado no puede exceder 50 caracteres.")]
        public string Estado { get; set; }

        public ICollection<DetalleFactura> DetallesFactura { get; set; } = new List<DetalleFactura>();
        public ICollection<Pagos> Pagos { get; set; } = new List<Pagos>();
    }
}

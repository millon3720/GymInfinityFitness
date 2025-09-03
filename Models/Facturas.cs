using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{
    public class Facturas
    {
        [Key]
        public int IdFactura { get; set; }

        [Required(ErrorMessage = "El cliente es obligatorio")]
        public int IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuarios? Usuario { get; set; }

        [Required(ErrorMessage = "La fecha de la factura es obligatoria.")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El total de la factura es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El total debe ser un valor positivo.")]
        public decimal Total { get; set; }
        public ICollection<DetalleFactura> DetallesFactura { get; set; } = new List<DetalleFactura>();
    }
}

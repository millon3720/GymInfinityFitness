using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{
    public class Facturas
    {
        [Key]
        public int IdFactura { get; set; }
        [Required]

        public int IdCliente { get; set; }
        [Required]
        public int IdUsuario { get; set; }
        [ForeignKey("IdUsuario")]
        public Usuarios Usuario { get; set; }

        public DateTime Fecha { get; set; }
        [Required]

        public decimal Total { get; set; }
        [Required]

        public string Estado { get; set; }
        public ICollection<DetalleFactura> DetallesFactura { get; set; }
        public ICollection<Pagos> Pagos { get; set; }
    }
}

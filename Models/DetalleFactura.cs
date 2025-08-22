using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{
    public class DetalleFactura
    {
        [Key]
        public int IdDetalleFactura { get; set; }
        [Required]
        public int IdFactura { get; set; }
        [ForeignKey("IdFactura")]
        public Facturas Factura { get; set; }
        public int IdProductoServicio { get; set; }
        [ForeignKey("IdProductoServicio")]
        public ProductosServicios ProductoServicio { get; set; }
        [Required]
        public int Cantidad { get; set; }
        [Required]
        public decimal PrecioUnitario { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{
    public class DetalleFactura
    {
        [Key]
        [Required(ErrorMessage = "El Id del detalle es obligatorio.")]
        public int IdDetalleFactura { get; set; }

        [Required(ErrorMessage = "Debe especificar la factura asociada.")]
        public int IdFactura { get; set; }

        [ForeignKey("IdFactura")]
        public Facturas Factura { get; set; }

        [Required(ErrorMessage = "Debe especificar el producto o servicio.")]
        public int IdProductoServicio { get; set; }

        [ForeignKey("IdProductoServicio")]
        public ProductosServicios ProductoServicio { get; set; }

        [Required(ErrorMessage = "Debe indicar la cantidad.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0.")]
        public int Cantidad { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0.")]
        public decimal Subtotal { get; set; }
    }
}

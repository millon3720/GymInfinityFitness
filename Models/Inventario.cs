using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{
    public class Inventario
    {
        [Key]
        public int IdInventario { get; set; }
        [Required]
        public int IdProductoServicio { get; set; }
        [ForeignKey("IdProductoServicio")]
        public ProductosServicios ProductoServicio { get; set; }
        [Required]        
        public int CantidadDisponible { get; set; }
        [Required]
        public int PuntoDeReorden { get; set; }
    }
}

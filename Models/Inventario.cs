using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{
    public class Inventario
    {
        [Key]
        public int IdInventario { get; set; }

        [Required(ErrorMessage = "El producto o servicio es obligatorio.")]
        public int IdProductoServicio { get; set; }

        [ForeignKey("IdProductoServicio")]
        public ProductosServicios ProductoServicio { get; set; }

        [Required(ErrorMessage = "La cantidad disponible es obligatoria.")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad disponible debe ser un número positivo.")]
        public int CantidadDisponible { get; set; }

        [Required(ErrorMessage = "El punto de reorden es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El punto de reorden debe ser un número positivo.")]
        public int PuntoDeReorden { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Tesina.Models
{
    public class ProductosServicios
    {
        [Key]
        public int IdProductoServicio { get; set; }

        [Required(ErrorMessage = "El nombre del producto o servicio es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El tipo de producto o servicio es obligatorio.")]
        [StringLength(50, ErrorMessage = "El tipo no puede exceder los 50 caracteres.")]
        public string Tipo { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero.")]
        public decimal Precio { get; set; }
        public ICollection<DetalleFactura>? DetallesFactura { get; set; }
        public Inventario? Inventario { get; set; }
    }
}

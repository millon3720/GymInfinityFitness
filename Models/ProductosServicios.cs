using System.ComponentModel.DataAnnotations;

namespace Tesina.Models
{
    public class ProductosServicios
    {

        [Key]
        public int IdProductoServicio { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Descripcion { get; set; }
        [Required]
        public string Tipo { get; set; }
        [Required]
        public decimal Precio { get; set; }
    }
}

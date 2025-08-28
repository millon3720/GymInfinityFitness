using System.ComponentModel.DataAnnotations;

namespace Tesina.Models
{
    public class Ejercicio
    {
        [Key]
        [Required(ErrorMessage = "El Id del ejercicio es obligatorio.")]
        public int IdEjercicio { get; set; }

        [Required(ErrorMessage = "El nombre del ejercicio es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción del ejercicio es obligatoria.")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El URL del video es obligatorio.")]
        [Url(ErrorMessage = "Debe ingresar una URL válida.")]
        public string VideoURL { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Tesina.Models
{
    public class Ejercicio
    {
        [Key]
        public int IdEjercicio { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Descripcion { get; set; }
        [Required]
        public string VideoURL { get; set; }

    }
}

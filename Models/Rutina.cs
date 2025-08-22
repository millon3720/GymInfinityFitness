using System.ComponentModel.DataAnnotations;
namespace Tesina.Models
{
    public class Rutina
    {
        [Key]
        public int IdRutina { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Descripcion { get; set; }
        [Required]
        public DateTime FechaCreacion { get; set; }
        public ICollection<RutinaEjercicio> RutinaEjercicio { get; set; } = new List<RutinaEjercicio>();

    }
}

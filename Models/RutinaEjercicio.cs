using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{

    public class RutinaEjercicio
    {
        [Key]
        public int IdRutinaEjercicio { get; set; }
        [Required]
        public int IdRutina { get; set; }
        [ForeignKey("IdRutina")]
        public Rutina Rutina { get; set; }
        [Required]
        public int IdEjercicio { get; set; }
        [ForeignKey("IdEjercicio")]
        public Ejercicio Ejercicio { get; set; }
        [Required]
        public int Series { get; set; }
        [Required]
        public int Repeticiones { get; set; }
        [Required]
        public int DescansoSegundos { get; set; }
        [Required]
        public string DiaSemana { get; set; }
    }
}

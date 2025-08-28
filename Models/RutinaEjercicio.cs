using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{

    public class RutinaEjercicio
    {
        [Key]
        public int IdRutinaEjercicio { get; set; }

        [Required(ErrorMessage = "La rutina es obligatoria.")]
        public int IdRutina { get; set; }

        [ForeignKey("IdRutina")]
        public Rutina Rutina { get; set; }

        [Required(ErrorMessage = "El ejercicio es obligatorio.")]
        public int IdEjercicio { get; set; }

        [ForeignKey("IdEjercicio")]
        public Ejercicio Ejercicio { get; set; }

        [Required(ErrorMessage = "Debe ingresar el número de series.")]
        [Range(1, 50, ErrorMessage = "El número de series debe estar entre 1 y 50.")]
        public int Series { get; set; }

        [Required(ErrorMessage = "Debe ingresar el número de repeticiones.")]
        [Range(1, 100, ErrorMessage = "El número de repeticiones debe estar entre 1 y 100.")]
        public int Repeticiones { get; set; }

        [Required(ErrorMessage = "Debe ingresar el tiempo de descanso en segundos.")]
        [Range(0, 600, ErrorMessage = "El descanso debe estar entre 0 y 600 segundos.")]
        public int DescansoSegundos { get; set; }

        [Required(ErrorMessage = "El día de la semana es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El nombre del día no puede exceder los 20 caracteres.")]
        public string DiaSemana { get; set; }
    }
}

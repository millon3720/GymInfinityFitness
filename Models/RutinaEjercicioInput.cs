using System.ComponentModel.DataAnnotations;

namespace Tesina.Models
{
    public class RutinaEjercicioInput
    {
        [Required(ErrorMessage = "El ejercicio es obligatorio.")]
        public int IdEjercicio { get; set; }

        [Required(ErrorMessage = "Debe ingresar el número de series.")]
        [Range(1, 50, ErrorMessage = "El número de series debe estar entre 1 y 50.")]
        public int Series { get; set; }

        [Required(ErrorMessage = "Debe ingresar el número de repeticiones.")]
        [Range(1, 100, ErrorMessage = "El número de repeticiones debe estar entre 1 y 100.")]
        public int Repeticiones { get; set; }

        [Required(ErrorMessage = "Debe ingresar el tiempo de descanso.")]
        [Range(0, 600, ErrorMessage = "El descanso debe estar entre 0 y 600 segundos.")]
        public int DescansoSegundos { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un día.")]
        [MaxLength(20, ErrorMessage = "El nombre del día no puede exceder los 20 caracteres.")]
        public string DiaSemana { get; set; }
    }

}

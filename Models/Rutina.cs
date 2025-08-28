using System.ComponentModel.DataAnnotations;
namespace Tesina.Models
{
    public class Rutina
    {
        [Key]
        public int IdRutina { get; set; }

        [Required(ErrorMessage = "El nombre de la rutina es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre de la rutina no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción de la rutina es obligatoria.")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La fecha de creación es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaCreacion { get; set; }

        public ICollection<RutinaEjercicio> RutinaEjercicio { get; set; } = new List<RutinaEjercicio>();
    }
}

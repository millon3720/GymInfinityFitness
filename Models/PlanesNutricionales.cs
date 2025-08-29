using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Tesina.Models
{
    public class PlanesNutricionales
    {
        [Key]
        public int IdPlan { get; set; }

        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        public int IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuarios? Usuario { get; set; }

        [Required(ErrorMessage = "La descripción del plan nutricional es obligatoria.")]
        [StringLength(200, ErrorMessage = "La descripción no puede exceder los 200 caracteres.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La fecha de asignación es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaAsignacion { get; set; }

        public ICollection<AlimentosPlanNutricional>? Alimentos { get; set; }
    }
}

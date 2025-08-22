using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models;

namespace Tesina.Models
{
    public class PlanesNutricionales
    {
        [Key]
        public int IdPlan { get; set; }
        [Required]
        public int IdUsuario { get; set; }
        [ForeignKey("IdUsuario")]
        public Usuarios Usuario { get; set; }
        [Required]
        public string Descripcion { get; set; }
        [Required]
        public DateTime FechaAsignacion { get; set; }
        public ICollection<AlimentosPlanNutricional> Alimentos { get; set; }

    }
}

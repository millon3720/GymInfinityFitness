using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tesina.Models;

namespace WebApplication1.Models
{
    public class AlimentosPlanNutricional
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdPlan { get; set; }
        [ForeignKey("IdPlan")]
        public PlanesNutricionales? PlanesNutricionales { get; set; }
        [Required]

        [MaxLength(20)]
        public string DiaSemana { get; set; }
        [Required]

        [MaxLength(20)]
        public string Comida { get; set; } 

        public TimeSpan? HoraEstimada { get; set; }
        [Required]

        [MaxLength(200)]
        public string Alimento { get; set; }
        [Required]
        [MaxLength(50)]
        public string Porciones { get; set; }

        [MaxLength(500)]
        public string? Comentarios { get; set; }

        
    }
}

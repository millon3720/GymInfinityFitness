using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tesina.Models;

namespace Tesina.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class AlimentosPlanNutricional
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El Id del plan es obligatorio.")]
        public int IdPlan { get; set; }

        [ForeignKey("IdPlan")]
        public PlanesNutricionales? PlanesNutricionales { get; set; }

        [Required(ErrorMessage = "Debe indicar el día de la semana.")]
        [MaxLength(20, ErrorMessage = "El día de la semana no puede superar los 20 caracteres.")]
        public string DiaSemana { get; set; }

        [Required(ErrorMessage = "Debe especificar la comida.")]
        [MaxLength(20, ErrorMessage = "El nombre de la comida no puede superar los 20 caracteres.")]
        public string Comida { get; set; }

        public TimeSpan? HoraEstimada { get; set; }

        [Required(ErrorMessage = "Debe indicar el alimento.")]
        [MaxLength(200, ErrorMessage = "El nombre del alimento no puede superar los 200 caracteres.")]
        public string Alimento { get; set; }

        [Required(ErrorMessage = "Debe indicar la porción.")]
        [MaxLength(50, ErrorMessage = "Las porciones no pueden superar los 50 caracteres.")]
        public string Porciones { get; set; }

        [MaxLength(500, ErrorMessage = "Los comentarios no pueden superar los 500 caracteres.")]
        public string? Comentarios { get; set; }
    }

}

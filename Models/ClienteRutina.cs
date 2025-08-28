using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{
    public class ClienteRutina
    {
        [Key]
        [Required(ErrorMessage = "El Id de la asignación es obligatorio.")]
        public int IdClienteRutina { get; set; }

        [Required(ErrorMessage = "Debe especificar el usuario al que se le asigna la rutina.")]
        public int IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuarios? Usuario { get; set; }

        [Required(ErrorMessage = "Debe especificar la rutina asignada.")]
        public int IdRutina { get; set; }

        [ForeignKey("IdRutina")]
        public virtual Rutina? Rutina { get; set; }

        [Required(ErrorMessage = "Debe indicar la fecha de asignación.")]
        [DataType(DataType.Date)]
        public DateTime FechaAsignacion { get; set; }

        [MaxLength(500, ErrorMessage = "Las observaciones no pueden exceder los 500 caracteres.")]
        public string? Observaciones { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{
    public class ClienteEvento
    {
        [Key]
        [Required(ErrorMessage = "El Id de la inscripción es obligatorio.")]
        public int IdClienteEvento { get; set; }

        [Required(ErrorMessage = "Debe especificar el usuario que se inscribe.")]
        public int IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuarios Usuario { get; set; }

        [Required(ErrorMessage = "Debe especificar el evento al que se inscribe.")]
        public int IdEvento { get; set; }

        [ForeignKey("IdEvento")]
        public Eventos Evento { get; set; }

        [Required(ErrorMessage = "Debe indicar la fecha de inscripción.")]
        [DataType(DataType.Date)]
        public DateTime FechaInscripcion { get; set; }
    }
}

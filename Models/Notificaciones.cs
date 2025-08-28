using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{
    public class Notificaciones
    {
        [Key]
        public int IdNotificacion { get; set; }

        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        public int IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuarios Usuario { get; set; }

        [Required(ErrorMessage = "El título de la notificación es obligatorio.")]
        [StringLength(100, ErrorMessage = "El título no puede exceder los 100 caracteres.")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "El mensaje de la notificación es obligatorio.")]
        [StringLength(500, ErrorMessage = "El mensaje no puede exceder los 500 caracteres.")]
        public string Mensaje { get; set; }

        [Required(ErrorMessage = "La fecha de envío es obligatoria.")]
        [DataType(DataType.DateTime)]
        public DateTime FechaEnvio { get; set; }
    }
}

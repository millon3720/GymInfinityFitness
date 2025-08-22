using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{
    public class Notificaciones
    {
        [Key]
        public int IdNotificacion { get; set; }
        [Required]
        public int IdUsuario { get; set; }
        [ForeignKey("IdUsuario")]
        public Usuarios Usuario { get; set; }
        [Required]
        public string Titulo { get; set; }
        [Required]
        public string Mensaje { get; set; }
        [Required]
        public DateTime FechaEnvio { get; set; }

    }
}

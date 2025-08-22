using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{
    public class ClienteEvento
    {
        [Key]
        public int IdClienteEvento { get; set; }
        [Required]
        public int IdUsuario { get; set; }
        [ForeignKey("IdUsuario")]
        public Usuarios Usuario { get; set; }
        [Required]
        public int IdEvento { get; set; }
        [ForeignKey("IdEvento")]
        public Eventos Evento { get; set; }
        [Required]
        public DateTime FechaInscripcion { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{
    public class ClienteRutina
    {
        [Key]
        public int IdClienteRutina { get; set; }
        [Required]
        public int IdUsuario { get; set; }
        [ForeignKey("IdUsuario")]
        public Usuarios Usuario { get; set; }
        [Required]
        public int IdRutina { get; set; }
        [ForeignKey("IdRutina")]
        public virtual Rutina Rutina { get; set; }
        [Required]
        public DateTime FechaAsignacion { get; set; }
        
        public string Observaciones { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{
    public class Asistencias
    {
        [Key]
        public int IdAsistencia { get; set; }
        [Required]
        public int IdUsuario { get; set; }
        [ForeignKey("IdUsuario")]
        public Usuarios Usuario { get; set; }
        [Required]
        public DateTime FechaIngreso { get; set; }
        [Required]
        public DateTime? FechaSalida { get; set; }
    }
}

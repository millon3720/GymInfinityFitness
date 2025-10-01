using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{
    public class Mensualidades
    {
        [Key]
        public int IdMensualidad { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public int IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuarios Usuario { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{
    public class Asistencias
    {
        [Key]
        public int IdAsistencia { get; set; }

        [Required(ErrorMessage = "Se debe especificar el usuario.")]
        public int IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        [Required(ErrorMessage = "Se debe proporcionar la información del usuario.")]
        public Usuarios Usuario { get; set; }

        [Required(ErrorMessage = "La fecha de ingreso es obligatoria.")]
        [DataType(DataType.DateTime)]
        public DateTime FechaIngreso { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? FechaSalida { get; set; }
    }
}

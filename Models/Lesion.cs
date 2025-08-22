using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{
    public class Lesion
    {
        [Key]

        public int IdLesion { get; set; }
        [Required]
        public int IdUsuario { get; set; }
        [ForeignKey("IdUsuario")]
        public Usuarios? Usuario { get; set; }
        [Required]
        public string Descripcion { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaDiagnostico { get; set; }

    }
}

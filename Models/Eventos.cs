using System.ComponentModel.DataAnnotations;

namespace Tesina.Models
{
    public class Eventos
    {
        [Key]
        [Required]
        public int IdEvento { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Descripcion { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime FechaEvento { get; set; }
    }
}

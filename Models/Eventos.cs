using System.ComponentModel.DataAnnotations;

namespace Tesina.Models
{
    public class Eventos
    {
        [Key]
        [Required(ErrorMessage = "El Id del evento es obligatorio.")]
        public int IdEvento { get; set; }

        [Required(ErrorMessage = "El nombre del evento es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción del evento es obligatoria.")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La fecha del evento es obligatoria.")]
        [DataType(DataType.DateTime)]
        public DateTime FechaEvento { get; set; }

        [Display(Name = "Imagen del evento")]
        public byte[]? Imagen { get; set; }
        public ICollection<ClienteEvento>? Inscripciones { get; set; }
    }
}

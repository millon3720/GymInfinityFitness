using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{

    public class UsuarioLogin
    {
        [Key]
        public int IdUsuarioLogin { get; set; }

        public int? IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuarios? InfoUsuario { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(50, ErrorMessage = "El usuario no puede exceder los 50 caracteres.")]
        public string Usuario { get; set; }

        [StringLength(100, ErrorMessage = "La contraseña no puede exceder los 100 caracteres.")]
        [DataType(DataType.Password)]
        public string? Contrasena { get; set; }
    }

}

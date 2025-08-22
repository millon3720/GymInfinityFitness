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
        [Required]
        public string Usuario {  get; set; }
        [Required]
        public string Contrasena { get; set; }
       
    }
}

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tesina.Models
{
    public class Usuarios
    {
        [Key]
        public int IdUsuario { get; set; }
        [Required]
        [StringLength(9, ErrorMessage = "La cédula debe tener un minimo de 9 caracteres.", MinimumLength = 9)]
        public string Cedula { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "El nombre completo no puede exceder los 100 caracteres.")]
        public string NombreCompleto { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }
        [Required]
        [EmailAddress]
        public string Correo { get; set; }
        [Required]
        [Phone]
        public string Telefono { get; set; }
        [Required]
        public string Direccion { get; set; }
        [Required]
        public string Rol { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime FechaRegistro { get; set; }
        public bool Estado { get; set; }

    }
}

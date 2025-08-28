using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tesina.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Usuarios
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "La cédula es obligatoria.")]
        [StringLength(9, ErrorMessage = "La cédula debe tener como minimo 9 caracteres.", MinimumLength = 9)]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre completo no puede exceder los 100 caracteres.")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [Phone(ErrorMessage = "El teléfono no tiene un formato válido.")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio.")]
        public string Rol { get; set; }

        [Required(ErrorMessage = "La fecha de registro es obligatoria.")]
        [DataType(DataType.DateTime)]
        public DateTime FechaRegistro { get; set; }

        public bool Estado { get; set; }
    }

}

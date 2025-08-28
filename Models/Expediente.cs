using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{
    public class Expediente
    {
        [Key]
        public int IdExpediente { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public int IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuarios? Usuario { get; set; }

        [Required(ErrorMessage = "La fecha de registro es obligatoria.")]
        public DateTime FechaRegistro { get; set; }

        [Required(ErrorMessage = "El peso es obligatorio.")]
        [Range(0, 500, ErrorMessage = "Ingrese un peso válido en Kg.")]
        public decimal PesoKg { get; set; }

        [Required(ErrorMessage = "La altura es obligatoria.")]
        [Range(0, 300, ErrorMessage = "Ingrese una altura válida en cm.")]
        public int AlturaCm { get; set; }

        [Required(ErrorMessage = "El IMC es obligatorio.")]
        [Range(0, 100, ErrorMessage = "Ingrese un IMC válido.")]
        public decimal IMC { get; set; }

        [Required(ErrorMessage = "El porcentaje de grasa es obligatorio.")]
        [Range(0, 100, ErrorMessage = "Ingrese un porcentaje válido.")]
        public decimal PorcentajeGrasa { get; set; }

        [Required(ErrorMessage = "El porcentaje muscular es obligatorio.")]
        [Range(0, 100, ErrorMessage = "Ingrese un porcentaje válido.")]
        public decimal PorcentajeMuscular { get; set; }

        [Required(ErrorMessage = "La medida de pecho es obligatoria.")]
        [Range(0, 200, ErrorMessage = "Ingrese una medida válida en cm.")]
        public decimal MedidaPecho { get; set; }

        [Required(ErrorMessage = "La medida de espalda es obligatoria.")]
        [Range(0, 200, ErrorMessage = "Ingrese una medida válida en cm.")]
        public decimal MedidaEspalda { get; set; }

        [Required(ErrorMessage = "La medida de cintura es obligatoria.")]
        [Range(0, 200, ErrorMessage = "Ingrese una medida válida en cm.")]
        public decimal MedidaCintura { get; set; }

        [Required(ErrorMessage = "La medida de cadera es obligatoria.")]
        [Range(0, 200, ErrorMessage = "Ingrese una medida válida en cm.")]
        public decimal MedidaCadera { get; set; }

        [Required(ErrorMessage = "El bícep derecho es obligatorio.")]
        [Range(0, 100, ErrorMessage = "Ingrese una medida válida en cm.")]
        public decimal BicepDerecho { get; set; }

        [Required(ErrorMessage = "El bícep izquierdo es obligatorio.")]
        [Range(0, 100, ErrorMessage = "Ingrese una medida válida en cm.")]
        public decimal BicepIzquierdo { get; set; }

        [Required(ErrorMessage = "La pierna derecha es obligatoria.")]
        [Range(0, 150, ErrorMessage = "Ingrese una medida válida en cm.")]
        public decimal PiernaDerecha { get; set; }

        [Required(ErrorMessage = "La pierna izquierda es obligatoria.")]
        [Range(0, 150, ErrorMessage = "Ingrese una medida válida en cm.")]
        public decimal PiernaIzquierda { get; set; }

        [Required(ErrorMessage = "La pantorrilla derecha es obligatoria.")]
        [Range(0, 100, ErrorMessage = "Ingrese una medida válida en cm.")]
        public decimal PantorrillaDerecha { get; set; }

        [Required(ErrorMessage = "La pantorrilla izquierda es obligatoria.")]
        [Range(0, 100, ErrorMessage = "Ingrese una medida válida en cm.")]
        public decimal PantorrillaIzquierda { get; set; }

        [MaxLength(500, ErrorMessage = "Las observaciones no pueden exceder los 500 caracteres.")]
        public string? Observaciones { get; set; }
    }
}


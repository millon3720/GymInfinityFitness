using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tesina.Models
{
    public class Expediente
    {
        [Key]
        public int IdExpediente { get; set; }
        [Required]
        public int IdUsuario { get; set; }
        [ForeignKey("IdUsuario")]
        public Usuarios? Usuario { get; set; }
        [Required]
        public DateTime FechaRegistro { get; set; }
        [Required]
        public decimal PesoKg { get; set; }
        [Required]
        public int AlturaCm { get; set; }
        [Required]
        public decimal IMC { get; set; }
        [Required]
        public decimal PorcentajeGrasa { get; set; }
        [Required]
        public decimal PorcentajeMuscular { get; set; }
        [Required]
        public decimal MedidaPecho { get; set; }
        [Required]
        public decimal MedidaEspalda { get; set; }
        [Required]
        public decimal MedidaCintura { get; set; }
        [Required]
        public decimal MedidaCadera { get; set; }
        [Required]
        public decimal BicepDerecho { get; set; }
        [Required]
        public decimal BicepIzquierdo { get; set; }
        [Required]
        public decimal PiernaDerecha { get; set; }
        [Required]
        public decimal PiernaIzquierda { get; set; }
        [Required]
        public decimal PantorrillaDerecha { get; set; }
        [Required]
        public decimal PantorrillaIzquierda { get; set; }
        public string? Observaciones { get; set; }
    }
}

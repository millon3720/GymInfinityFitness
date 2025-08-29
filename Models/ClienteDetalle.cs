using Tesina.Models;
using System.Collections.Generic;

namespace Tesina.Models
{
    public class ClienteDetalle
    {
        public Usuarios Cliente { get; set; }
        public List<Expediente> Expedientes { get; set; }

        public ClienteRutina? ClienteRutina { get; set; }      
        public Rutina? Rutina { get; set; }                    
        public List<RutinaEjercicio>? RutinaEjercicios { get; set; }
        public List<Lesion>? Lesion { get; set; }
        public List<PlanesNutricionales>? PlanesNutricionales { get; set; }
        public List<AlimentosPlanNutricional>? AlimentosPlanNutricional { get; set; }
        public List<Asistencias>? Asistencias { get; set; }
        public List<Facturas>? Facturas { get; set; }
       

    }
}

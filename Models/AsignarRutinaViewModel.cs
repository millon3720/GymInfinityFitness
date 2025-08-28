namespace Tesina.Models
{
    public class AsignarRutinaViewModel
    {
        public int IdUsuario { get; set; } // usuario al que se le asigna
        public DateTime FechaAsignacion { get; set; } = DateTime.Now;
        public string Observaciones { get; set; }

        public int RutinaSeleccionada { get; set; } // id de la rutina elegida
        public List<RutinaItemViewModel> RutinasDisponibles { get; set; } = new();
    }
}

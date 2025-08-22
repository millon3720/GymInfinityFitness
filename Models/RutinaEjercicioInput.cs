namespace WebApplication1.Models
{
    public class RutinaEjercicioInput
    {
        public int IdEjercicio { get; set; }
        public int Series { get; set; }
        public int Repeticiones { get; set; }
        public int DescansoSegundos { get; set; }
        public string DiaSemana { get; set; }
    }
}

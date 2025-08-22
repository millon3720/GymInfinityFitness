using Tesina.Models;

namespace WebApplication1.Models
{
    public class RutinaCreateViewModel
    {
        public Rutina Rutina { get; set; }
        public List<RutinaEjercicioInput> Ejercicios { get; set; } = new List<RutinaEjercicioInput>();
    }  

}

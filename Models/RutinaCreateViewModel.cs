using Tesina.Models;

namespace Tesina.Models
{
    public class RutinaCreateViewModel
    {
        public Rutina Rutina { get; set; }
        public List<RutinaEjercicioInput> Ejercicios { get; set; } = new List<RutinaEjercicioInput>();
    }  

}

using Tesina.Models;

namespace Tesina.Models
{
    public class PlanAlimenticioViewModel
    {
        public PlanesNutricionales Plan { get; set; } = new PlanesNutricionales();
        public List<AlimentosPlanNutricional> Alimentos { get; set; } = new List<AlimentosPlanNutricional>();

    }
}

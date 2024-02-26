#nullable disable
using System.ComponentModel.DataAnnotations.Schema;

namespace PalettaPolizeiPro.Services.Simulation
{
    public class SimulationPaletta 
    {
        public string Identifier { get; set; }
        public int Loop { get; set; }
        public int PredefiniedCycle { get; set; }
        public int ActualCycle { get; set; }
        public string EngineNumber { get; set; }
        public DateTime ReadTime { get; set; }

        [NotMapped]
        public float ServicePercentage
        {
            get
            {
                if (PredefiniedCycle == 0 || ActualCycle == 0) { return 0; }
                float p = ActualCycle / (float)PredefiniedCycle * 100;
                return p;
            }
        }
    }
}

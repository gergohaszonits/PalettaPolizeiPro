using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionLineSimulation.Simulation
{
    public class SimulationPaletta
    {
        public required string Name { get; set; }
        public int PredefiniedCycle { get; set; }
        public int ActualCycle { get; set; }
        public int Loop { get; set; }
        public string? Engine { get; set; } 
        public int OutsortedRound { get; set; }
        public bool Outsorted { get; set; }

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

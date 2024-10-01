

namespace ProductionLineSimulation.Simulation
{
    public class SimulationStation
    {
        public required string Name { get; set; }
        public int DB { get; set; }
        public int Loop { get; set; }
        public required SimulationPlcLayer Plc { get; set; }
        public SimulationPaletta? Paletta { get; protected set; }
        public virtual void PalettaIn(SimulationPaletta paletta) { }
        public virtual void PalettaOut() { }

    }
}

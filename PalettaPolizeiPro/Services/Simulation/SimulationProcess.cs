
using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Data.Palettas;

namespace PalettaPolizeiPro.Services.Simulation
{
    // FIGYELEM!! CSAK DUMMY PLC VEL SZABAD HASZNALNI
    public class SimulationProcess : IUpdatable
    {
        public List<Loop> Loops { get; private set; }
        
        public SimulationProcess(List<Loop> loops)
        {
            Loops = loops;
        }
        public async Task Update()
        {
            //implement logic
        }
    }
}

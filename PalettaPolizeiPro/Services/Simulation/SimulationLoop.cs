using PalettaPolizeiPro.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalettaPolizeiPro.Services.Simulation
{
    public class SimulationLoop
    {
        public required List<Station> QueryStations { get; set; }
        public required List<Station> CheckinStations { get; set; }

        public required List<SimulationPaletta> Palettas { get; set; }
    }
}

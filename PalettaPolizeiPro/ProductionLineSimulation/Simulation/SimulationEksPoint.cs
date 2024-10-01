using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductionLineSimulation.Simulation
{
    public class SimulationEksPoint
    {
        public SimulationEksPoint(SimulationPlcLayer plc) 
        {
            Plc = plc;
        }
        private SimulationPlcLayer Plc;

        public string? GetKeyId()
        {
            return null;
        }
        
        public string? GetWerkerId()
        {         
            return null;
        }

        public void SetKeyId(string id)
        {
        }

        public void SetWerkerId(string id)
        {
        }
    }
}

using ProductionLineSimulation.Communication.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductionLineSimulation.Communication
{
    public class LinePacketWriteRequest : LinePacket
    {
        public required int Db { get; set; }
        public required int Index { get; set; }
        public bool BitValue { get; set; }
        public byte? Bit { get; set; } 
        public byte[]? Data { get; set; }

    }
}

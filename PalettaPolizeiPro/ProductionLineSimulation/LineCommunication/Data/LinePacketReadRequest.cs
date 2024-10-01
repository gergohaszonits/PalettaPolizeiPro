using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductionLineSimulation.Communication.Data
{
    public class LinePacketReadRequest : LinePacket
    {
        public required int Db { get; set; }
        public required int Index { get; set; }
        public int? Bit { get; set; }
        public int Size { get; set; }

    }
}

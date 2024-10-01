using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductionLineSimulation.Communication.Data
{
    public class LinePacket
    {
        public required string Ip { get; set; }
        public required int Rack { get; set; }
        public required int Slot { get; set; }
        public required LinePacketIntent Intent { get; set; } // read, write, connection
        public required DateTime SentTime { get; set; }

    }
}

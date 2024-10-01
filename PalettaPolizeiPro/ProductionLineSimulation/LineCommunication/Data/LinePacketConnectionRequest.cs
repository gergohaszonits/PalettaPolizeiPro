using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductionLineSimulation.Communication.Data
{
    public class LinePacketConnectionRequest : LinePacket
    {
        public LinePacketConnectionIntent ConnectionIntent { get; set; } // conn or disconn

    }
}

using ProductionLineSimulation.Communication.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ProductionLineSimulation.LineCommunication.Data
{
    public class LinePacketReceivedEventArgs
    {
        public required NetworkStream Stream { get; set; }
        public required LinePacket Packet { get; set; }
    }
}

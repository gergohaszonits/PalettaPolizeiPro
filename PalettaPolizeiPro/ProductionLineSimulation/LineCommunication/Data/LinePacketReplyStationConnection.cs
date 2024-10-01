using ProductionLineSimulation.LineCommunication.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductionLineSimulation.Communication.Data
{
    public class LinePacketReplyStationConnection : LinePacketReply
    {
        public required LinePacketReplyConnectionState State { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductionLineSimulation.LineCommunication.Data
{
    public class LinePacketReplyRead : LinePacketReply
    {
        public byte[]? Data { get; set; }
        public bool BitValue { get; set; }

    }
}

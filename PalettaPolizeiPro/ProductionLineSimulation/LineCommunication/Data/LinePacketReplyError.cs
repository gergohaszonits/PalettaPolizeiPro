using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductionLineSimulation.LineCommunication.Data
{
    public class LinePacketReplyError : LinePacketReply
    {
        public required string ErrorText { get; set; }
    }
}

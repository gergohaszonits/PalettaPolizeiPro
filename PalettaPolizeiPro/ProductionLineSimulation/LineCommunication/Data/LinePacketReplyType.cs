using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductionLineSimulation.LineCommunication.Data
{
    public enum LinePacketReplyType
    {
        DataBlock,
        Error,
        Connection,
        Eks,
        Ok
    }
}

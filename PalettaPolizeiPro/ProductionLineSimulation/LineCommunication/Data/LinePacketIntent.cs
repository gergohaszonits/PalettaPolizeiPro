﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductionLineSimulation.Communication.Data
{

    public enum LinePacketIntent
    {
        Read,
        Write,
        Connection,
        GetStationState,
    }

}

﻿using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Services.PLC;

namespace PalettaPolizeiPro.Services.PalettaControl
{
    public interface IPalettaControlService
    {
        PalettaProperty GetProperty(Station station);
        QueryState GetQueryState(Station station);
        void SetQueryState(QueryState state, Station station);
        public static abstract IPalettaControlService GetInstance();
        public static abstract List<IPLCLayer>? PLCs { get; set; }
        public static abstract void Init(List<IPLCLayer> plcs);
       
    }
}

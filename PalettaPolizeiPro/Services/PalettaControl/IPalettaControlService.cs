﻿using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Data.Stations;
using PalettaPolizeiPro.Services.PLC;

namespace PalettaPolizeiPro.Services.PalettaControl
{
    public interface IPalettaControlService
    {
        PalettaProperty? GetProperty(Station station);
        QueryState GetQueryState(Station station);
        List<IPLCLayer> GetPlcs();
        List<PlcStationGroups> GetPlcStationGroups();
        void Init(List<Station> stations);
        bool IsParentPlcConnected(Station station);
        PalettaProperty? GetCachedProperty(Station station);
        QueryState? GetCachedQueryState(Station station);
        void PalettaGo(Station staiton);
        void PalettaOut(Station station);
        void OperationStatusOff(Station station);
        void OperationStatusOn(Station station);


    }
}

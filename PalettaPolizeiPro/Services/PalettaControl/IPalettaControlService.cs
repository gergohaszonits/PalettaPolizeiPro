using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Data.Stations;
using PalettaPolizeiPro.Services.PLC;

namespace PalettaPolizeiPro.Services.PalettaControl
{
    public interface IPalettaControlService
    {
        PalettaProperty? GetProperty(Station station);
        QueryState GetQueryState(Station station);
        void SetQueryState(QueryState state, Station station);
        List<IPLCLayer> GetPlcs();
        List<PlcStationGroups> GetPlcStationGroups();
        void Init(List<Station> stations);

    }
}

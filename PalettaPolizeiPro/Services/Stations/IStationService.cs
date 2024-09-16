using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using PalettaPolizeiPro.Data.Stations;
using PalettaPolizeiPro.Services.PLC;

namespace PalettaPolizeiPro.Services.Stations
{
    public interface IStationService
    {
        EventHandler<StationsChangedArgs> OnStationChange { get; }
        Station AddStation(Station station);
        void RemoveStation(Station station);
        void ModifyStation(Station station);
        List<Station> GetAll();
        List<Station> GetWhere(Func<Station, bool> predicate);
        Station? Get(Func<Station, bool> predicate);
    }
}

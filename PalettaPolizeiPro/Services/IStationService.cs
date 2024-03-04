using PalettaPolizeiPro.Data;

namespace PalettaPolizeiPro.Services
{
    public interface IStationService
    {
        void AddStation(Station station);
        void RemoveStation(Station station);
        void ModifyStation(Station station);
    }
}

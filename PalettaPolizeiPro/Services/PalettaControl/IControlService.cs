using PalettaPolizeiPro.Data.EKS;
using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Data.Stations;
using PalettaPolizeiPro.Services.PLC;

namespace PalettaPolizeiPro.Services.PalettaControl
{
    public interface IControlService
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

        Eks? GetEks(Station station);
        Eks? GetCachedEks(Station station);

        List<Eks> GetStoredEksList();
        List<Eks> GetStoredEksList(Func<Eks, bool> predicate);
        Eks? EksFirstOrNull(Func<Eks, bool> predicate);


        List<Paletta> GetPalettas();
        List<Paletta> GetPalettas(Func<Paletta,bool> predicate);
        List<Paletta> GetPalettasWithLastProperty(Func<Paletta, bool> predicate);
        List<Paletta> GetPalettasWithLastProperty();
        List<PalettaProperty> GetPalettaProperties();
        List<PalettaProperty> GetPalettaProperties(Func<PalettaProperty, bool> predicate);

        void CreatePaletta(Paletta paletta);

    }
}

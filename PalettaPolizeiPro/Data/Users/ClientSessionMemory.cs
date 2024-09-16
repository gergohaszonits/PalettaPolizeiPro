using PalettaPolizeiPro.Data.Stations;

namespace PalettaPolizeiPro.Data.Users
{
    public class ClientSessionMemory
    {
        public StationType StationEditorLastSelectedType { get; set; } = StationType.Query;
    }
}

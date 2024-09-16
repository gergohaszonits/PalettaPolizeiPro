namespace PalettaPolizeiPro.Data.Stations
{

    public class StationsChangedArgs
    {
        public required Station Station { get; set; }
        public required ChangeState State { get; set; }

    }
}

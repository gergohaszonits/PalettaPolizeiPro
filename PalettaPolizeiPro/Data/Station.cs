namespace PalettaPolizeiPro.Data
{
    public class Station
    {
        public required string IP { get; set; }
        public required int Rack { get; init; }
        public required int Slot { get; init; }
        public bool IsStationOn { get; set; }
        public required string Name { get; init; }
        public int Loop { get; init; }
        public int DB { get; init; }
    }
}

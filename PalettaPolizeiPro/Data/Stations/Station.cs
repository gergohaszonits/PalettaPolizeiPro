namespace PalettaPolizeiPro.Data.Stations
{
    public class Station : EntityObject
    {
        public required string Ip { get; set; }
        public required int Rack { get; set; }
        public required int Slot { get; set; }
        public bool IsStationOn { get; set; }
        public required string Name { get; set; }
        public int Loop { get; set; }
        public int DB { get; set; }
        public string? StationPcIp { get; set; }
        public StationType StationType { get; set; }

        public void Update(Station station)
        {
            if (station != null)
            {
                foreach (var property in typeof(Station).GetProperties())
                {
                    if (property.CanWrite)
                    {
                        property.SetValue(this, property.GetValue(station));
                    }
                }
            }
        }
    }
}

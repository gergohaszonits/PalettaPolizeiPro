namespace PalettaPolizeiPro.Data.Palettas
{
    public class Loop
    {
        public required string Name { get; set; } 
        public required List<Station> QueryStations { get; set; }
        public required List<Station> CheckinStations { get; set; }
        public required List<Paletta> Palettas { get; set; }

        public int StationCount { get { return QueryStations.Count + CheckinStations.Count; } }

    }
}

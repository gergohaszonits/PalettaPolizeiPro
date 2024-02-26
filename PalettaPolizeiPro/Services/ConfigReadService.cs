using PalettaPolizeiPro.Data;

namespace PalettaPolizeiPro.Services
{
    public class ConfigReadService
    {
        private static List<Station>? _queryStations { get; set; }
        private static List<Station>? _checkinStations { get; set; }
        private static List<Station>? _EksStations { get; set; }

        public List<Station> GetEksStations()
        {
            if (_EksStations is null)
            {
                _EksStations = ReadStations(Path.Combine(Environment.CurrentDirectory!, Path.Combine("Config",$"EKSinterface.csv")));
            }
            return _EksStations;
        }
        public List<Station> GetCheckinStations()
        {
            if (_checkinStations is null)
            {
                _checkinStations = ReadStations(Path.Combine(Environment.CurrentDirectory!, Path.Combine("Config",$"CheckInPoints.csv")));
            }
            return _checkinStations;
        }
        public List<Station> GetQueryStations()
        {
            if (_queryStations is null)
            {
                _queryStations = ReadStations(Path.Combine(Environment.CurrentDirectory!, Path.Combine("Config",$"QueryPoints.csv")));
            }
            return _queryStations;
        }
        private static List<Station> ReadStations(string route)
        {
            List<Station> stations = new List<Station>();

            string[] lines = File.ReadAllLines(route);

            foreach (string line in lines)
            {
                if (line.StartsWith("//")) { continue; }
                string[] parameter = line.Split(';');
                Station temp = new Station()
                {
                    Name = parameter[0],
                    IP = parameter[2],
                    Rack = Convert.ToInt32(parameter[3]),
                    Slot = Convert.ToInt32(parameter[4]),
                    DB = Convert.ToInt32(parameter[5]),
                    Loop = Convert.ToInt32(parameter[6].Substring(1))
                };
                stations.Add(temp);
            }
            return stations;
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Data.Stations;
using PalettaPolizeiPro.Database;
using System;

namespace PalettaPolizeiPro.Services.Stations
{
    public class StationService : IStationService
    {
        private StationService() { }    
        private static StationService _instance = new StationService(); 
        public static StationService GetInstance() { return _instance; }    
        public EventHandler<StationsChangedArgs> OnStationChange { get; set; } = delegate { };

        public Station AddStation(Station station)
        {
            using (var context = new DatabaseContext())
            {
                context.Stations.Add(station);
                context.SaveChanges();
                OnStationChange.Invoke(this, new StationsChangedArgs
                {
                    State = ChangeState.Added,
                    Station = station
                });
                return station;
            }
        }
        public Station? Get(Func<Station, bool> predicate)
        {
            using (var context = new DatabaseContext())
            {
                return context.Stations.AsNoTracking().FirstOrDefault(predicate);
            }
        }

        public List<Station> GetAll()
        {
            using (var context = new DatabaseContext())
            {
                return context.Stations.AsNoTracking().OrderBy(x => x.Name).ToList();
            }
        }

        public List<Station> GetWhere(Func<Station, bool> predicate)
        {
            using (var context = new DatabaseContext())
            {
                return context.Stations.AsNoTracking().OrderBy(x=>x.Name).Where(predicate).ToList();
            }
        }

        public void ModifyStation(Station station)
        {
            using (var context = new DatabaseContext())
            {
                context.Entry(station).State = EntityState.Modified;
                context.SaveChanges();
                OnStationChange.Invoke(this, new StationsChangedArgs
                {
                    State = ChangeState.Modified,
                    Station = station
                });
            }
        }

        public void RemoveStation(Station station)
        {
            using (var context = new DatabaseContext())
            {
                context.Stations.Remove(station);
                context.SaveChanges();
                OnStationChange.Invoke(this, new StationsChangedArgs
                {
                    State = ChangeState.Removed,
                    Station = station
                });
            }
        }

        
    }
}

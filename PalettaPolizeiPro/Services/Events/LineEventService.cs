using Microsoft.EntityFrameworkCore;
using PalettaPolizeiPro.Data.Events;
using PalettaPolizeiPro.Database;

namespace PalettaPolizeiPro.Services.Events
{
    public class LineEventService
    {
        private LineEventService() { }
        private static LineEventService _instance = new LineEventService();
        public static LineEventService GetInstance()
        {
            return _instance;
        }

        public event EventHandler<QueryEventArgs> QueryEvent = delegate { };
        public QueryEventArgs LastQueryEvent { get; private set; }

        public event EventHandler<CheckEventArgs> CheckEvent = delegate { };
        public CheckEventArgs LastCheckEvent { get; private set; }

        public event EventHandler<EksEventArgs> EksEvent = delegate { };
        public EksEventArgs LastEksEvent { get; private set; }

        public List<CheckEventArgs> GetCheckEvents()
        {
            using (var context = new DatabaseContext())
            {
                return context.CheckEvents.OrderByDescending(x=>x.Time).Include(x => x.Station).Include(x => x.Property).ToList();
            }
        }
        public List<CheckEventArgs> GetCheckEvents(Func<CheckEventArgs,bool> predicate)
        {
            using (var context = new DatabaseContext())
            {
                return context.CheckEvents.OrderByDescending(x => x.Time).Include(x => x.Station).Include(x => x.Property).Where(predicate).ToList();
            }
        }
        public List<QueryEventArgs> GetQueryEvents()
        {
            using (var context = new DatabaseContext())
            {
                return context.QueryEvents.OrderByDescending(x => x.Time).Include(x => x.Station).Include(x => x.State).ToList();
            }
        }
        public List<QueryEventArgs> GetQueryEvents(Func<QueryEventArgs, bool> predicate)
        {
            using (var context = new DatabaseContext())
            {
                return context.QueryEvents.OrderByDescending(x => x.Time).Include(x => x.Station).Include(x => x.State).Where(predicate).ToList();
            }
        }
        public List<EksEventArgs> GetEksEvents()
        {
            using (var context = new DatabaseContext())
            {
                return context.EksEvents.OrderByDescending(x => x.Time).Include(x => x.Station).Include(x => x.Eks).ToList();
            }
        }
        public List<EksEventArgs> GetEksEvents(Func<EksEventArgs, bool> predicate)
        {
            using (var context = new DatabaseContext())
            {
                return context.EksEvents.OrderByDescending(x => x.Time).Include(x => x.Station).Include(x => x.Eks).Where(predicate).ToList();
            }
        }

        public Task NewQueryEvent(QueryEventArgs e)
        {
            return Task.Run(() => 
            {
                using (var context = new DatabaseContext())
                {
                    context.QueryEvents.Add(e);
                    context.SaveChanges();
                    LastQueryEvent = e;
                    QueryEvent.Invoke(this,e);                   
                }     
            });
        }
        public Task NewCheckEvent(CheckEventArgs e)
        {
            return Task.Run(() => 
            {
                using (var context = new DatabaseContext())
                {
                    context.CheckEvents.Add(e);
                    context.SaveChanges();
                    LastCheckEvent  = e;
                    CheckEvent.Invoke(this, e);
                }

            });
        }
        public Task NewEksEvent(EksEventArgs e)
        {
            return Task.Run(() => 
            {
                using (var context = new DatabaseContext())
                {
                    context.EksEvents.Add(e);
                    context.SaveChanges();
                    LastEksEvent = e;
                    EksEvent.Invoke(this, e);
                }

            });

        }
    }
}

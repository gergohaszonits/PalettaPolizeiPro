using Microsoft.EntityFrameworkCore;
using PalettaPolizeiPro.Data.LineEvents;
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

        public event EventHandler<EksConfirmEventArgs> EksConfirmEvent = delegate { };
        public EksConfirmEventArgs LastEksConfirmEvent { get; private set; }

        public List<CheckEventArgs> GetCheckEvents()
        {
            using (var context = new DatabaseContext())
            {
                return context.CheckEvents.OrderByDescending(x => x.Time).Include(x => x.Station).Include(x => x.Property).ToList();
            }
        }
        public List<CheckEventArgs> GetCheckEvents(Func<CheckEventArgs, bool> predicate)
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
                return context.EksEvents.OrderByDescending(x => x.Time).Include(x => x.Station).ToList();
            }
        }
        public List<EksEventArgs> GetEksEvents(Func<EksEventArgs, bool> predicate)
        {
            using (var context = new DatabaseContext())
            {
                return context.EksEvents.OrderByDescending(x => x.Time).Include(x => x.Station).Where(predicate).ToList();
            }
        }

        public Task NewQueryEvent(QueryEventArgs e)
        {
             return Task.Run(() =>
            {
                try
                {
                    using (var context = new DatabaseContext())
                    {
                        var paletta = context.Palettas.FirstOrDefault(x => x.Identifier == e.State.PalettaName);             
                        if(paletta is not null)
                        {
                            if (e.State.ControlFlag == 4)
                            {
                                paletta.IsOut = true;
                                paletta.Marked = false;
                                context.Entry(paletta).State = EntityState.Modified;
                                context.SaveChanges();
                            }
                            e.State.PalettaId = paletta.Id;
                        }

                       
                        var temp = e.Station;
                        e.Station = null;
                        context.QueryEvents.Add(e);
                        context.SaveChanges();
                        e.Station = temp;

                        LastQueryEvent = e;
                        QueryEvent.Invoke(this, e);
                    }
                }
                catch (Exception ex) { LogService.LogException(ex); }
            });
        }
        public Task NewCheckEvent(CheckEventArgs e)
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var context = new DatabaseContext())
                    {
                        var paletta = context.Palettas.FirstOrDefault(x => x.Identifier == e.Property.Identifier);
                        if (paletta is null)
                        {
                            paletta = new Data.Palettas.Paletta
                            {
                                Identifier = e.Property.Identifier,
                                Loop = e.Station.Loop,

                            };
                            context.Palettas.Add(paletta); // mentunk hogy kapjon ID t az entitiytol
                            context.SaveChanges();

                        }
                        else
                        {
                            if (paletta.IsOut)
                            {
                                paletta.IsOut = false;
                                context.Entry(paletta).State = EntityState.Modified;
                                context.SaveChanges();
                            }
                        }

                        e.Property.PalettaId = paletta.Id;
                        var temp = e.Station;
                        e.Station = null;
                        context.CheckEvents.Add(e);
                        context.SaveChanges();
                        e.Station = temp;
                        LastCheckEvent = e;
                        CheckEvent.Invoke(this, e);
                    }
                }
                catch (Exception ex) { LogService.LogException(ex); }
            });
        }
        public Task NewEksEvent(EksEventArgs e)
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var context = new DatabaseContext())
                    {
                        var temp = e.Station;
                        e.Station = null;
                        context.EksEvents.Add(e);
                        context.SaveChanges();
                        e.Station = temp;
                        LastEksEvent = e;
                        EksEvent.Invoke(this, e);
                    }
                }
                catch (Exception ex) { LogService.LogException(ex); }
            });

        }
        public Task NewEksConfirmEvent(EksConfirmEventArgs e)
        {
            return Task.Run(() =>
            {
                try
                {
                    LastEksConfirmEvent = e;
                    EksConfirmEvent.Invoke(this, e);
                }
                catch (Exception ex) { LogService.LogException(ex); }
            });
        }
    }
}

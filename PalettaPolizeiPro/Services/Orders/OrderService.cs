using Microsoft.EntityFrameworkCore;
using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Data.Stations;
using PalettaPolizeiPro.Data.Users;
using PalettaPolizeiPro.Database;
using System;
using static MudBlazor.Colors;

namespace PalettaPolizeiPro.Services.Orders
{
    public class OrderService : IOrderService
    {
        public static event EventHandler<OrderEventArgs> OrdersChanged = delegate { };
        private object _orderLock = new object();

        public void AddOrUpdate(Order order)
        {

            var state = order.Id == 0 ? ChangeState.Added : ChangeState.Modified;
            List<Paletta> temppalettas;
            User tempuser;
            if (state == ChangeState.Added)
            {
                using (var context = new DatabaseContext())
                {
                    temppalettas = order.ScheduledPalettas;
                    tempuser = order.User;
                    order.User = null;
                    order.ScheduledPalettas = new List<Paletta>();
                    context.Orders.Add(order);
                    context.SaveChanges();
                }
                order.ScheduledPalettas = temppalettas;
                order.User = tempuser;
                using (var context = new DatabaseContext())
                {
                    foreach (var p in order.ScheduledPalettas)
                    {
                        context.OrderPalettaSchedules.Add(new OrderPalettaScheduled
                        {
                            OrderId = order.Id,
                            PalettaId = p.Id
                        });

                    }
                    context.SaveChanges();

                }
            }
            else if (state == ChangeState.Modified)
            {
               

                using (var context = new DatabaseContext())
                {
                    foreach (var p in order.FinishedPalettas)
                    {
                        if (context.OrderPalettaFinishes.FirstOrDefault(x => x.PalettaId == p.Id && x.OrderId == order.Id) is null)
                        {
                            context.OrderPalettaFinishes.Add(new OrderPalettaFinished
                            {
                                OrderId = order.Id,
                                PalettaId = p.Id
                            });
                        }
                    }
                    context.SaveChanges();
                }

                using (var context = new DatabaseContext())
                {
                    context.Entry(order).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }




            OrdersChanged.Invoke(this, new OrderEventArgs
            {
                State = state,
                Order = order,
                Time = DateTime.Now
            });
        }

        public List<Order> GetAll()
        {
            using (var context = new DatabaseContext())
            {
                return context.Orders.AsNoTracking().OrderByDescending(x => x.ScheduledTime).Include(x => x.FinishedPalettas).Include(x => x.OrderPalettaFinishes).Include(x => x.OrderPalettaSchedules).Include(x => x.ScheduledPalettas).Include(x => x.User).ToList();
            }
        }

        public List<Order> GetWhere(Func<Order, bool> predicate)
        {
            using (var context = new DatabaseContext())
            {
                return context.Orders.AsNoTracking().OrderByDescending(x => x.ScheduledTime).Include(x => x.OrderPalettaFinishes).Include(x => x.OrderPalettaSchedules).Include(x => x.FinishedPalettas).Include(x => x.ScheduledPalettas).Include(x => x.User).Where(predicate).ToList();
            }
        }

        public void Remove(Order order)
        {
            using (var context = new DatabaseContext())
            {
                context.Orders.Remove(order);
                context.SaveChanges();
            }
            OrdersChanged.Invoke(this, new OrderEventArgs
            {
                State = ChangeState.Removed,
                Order = order,
                Time = DateTime.Now
            });
        }
    }
}

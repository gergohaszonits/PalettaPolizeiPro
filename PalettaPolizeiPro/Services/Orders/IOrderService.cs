using PalettaPolizeiPro.Data.Palettas;

namespace PalettaPolizeiPro.Services.Orders
{
    public interface IOrderService
    {
        void AddOrUpdate(Order order);
        void Remove(Order order);
        List<Order> GetAll();
        List<Order> GetWhere(Func<Order,bool> predicate);
        public void Notify(OrderEventArgs args);
    }
}

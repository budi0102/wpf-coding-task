using System.Collections.Generic;
using System.Threading.Tasks;
using WpfApp.Models;

namespace WpfApp.Services
{
    public interface IOrderService
    {
        IEnumerable<Order> LoadFile(string file);

        Task<bool> SaveFileAsync(IEnumerable<Order> orders, string file);

        string ValidateOrder(Order order);

        string ValidateOrders(IEnumerable<Order> orders);

        OrderStatus SendOrder(Order order);
    }
}

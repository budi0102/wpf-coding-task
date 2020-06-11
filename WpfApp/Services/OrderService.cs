using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WpfApp.Models;

namespace WpfApp.Services
{
    public class OrderService : IOrderService
    {
        private static readonly Random rand = new Random();

        public IEnumerable<Order> LoadFile(string file)
        {
            string line;
            using (var reader = File.OpenText(file))
            {
                line = reader.ReadLine(); // skip first line
                while ((line = reader.ReadLine()) != null)
                {
                    Order newOrder = Order.Parse(line);
                    yield return newOrder;
                }
            }
        }

        public async Task<bool> SaveFileAsync(IEnumerable<Order> orders, string file)
        {
            try
            {
                using (StreamWriter outputFile = new StreamWriter(file))
                {
                    await outputFile.WriteLineAsync(Order.CSVHeaders());
                    foreach (var order in orders)
                    {
                        await outputFile.WriteLineAsync(order.ToCSV());
                    }
                }
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public string ValidateOrder(Order order)
        {
            order.ForceValidate();
            return order?.Error;
        }

        public string ValidateOrders(IEnumerable<Order> orders)
        {
            if(!(orders?.Any() ?? false))
            {
                return null;
            }

            int i = 0;
            List<string> errors = new List<string>();
            foreach(var order in orders)
            {
                order.ForceValidate();
                string error = order.Error;
                if(!string.IsNullOrEmpty(error))
                {
                    errors.Add($"{i}. {error}");
                }
            }
            return string.Join(Environment.NewLine, errors);
        }

        /// <summary>
        /// Send order will return the status after 5 seconds
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public OrderStatus SendOrder(Order order)
        {
            //var max = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().Max();
            //int index = rand.Next(max);
            int index = rand.Next(3);

            Thread.Sleep(5000); // simulate long process

            switch (index)
            {
                case 0: return OrderStatus.Rejected;
                case 1: return OrderStatus.NotSent;
                case 2:
                default: return OrderStatus.Sent;
            }
        }

        
    }
}

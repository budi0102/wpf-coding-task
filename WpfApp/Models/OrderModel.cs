using System;

namespace WpfApp.Models
{
    public class OrderModel : Order
    {
        private bool isEnabled;
        private OrderStatus status;

        public OrderModel()
            : base()
        {
            this.isEnabled = true;
            this.status = OrderStatus.None;
        }

        public OrderModel(string tradingAccount, Guid? userID, string instrumentCode, string side, decimal? price, long? quantity, bool isEnabled = true, OrderStatus status = OrderStatus.None)
            : base(tradingAccount, userID, instrumentCode,side,price,quantity)
        {
            this.isEnabled = isEnabled;
            this.status = status;
        }

        public OrderModel(Order other)
            : this(other.TradingAccount, other.UserID, other.InstrumentCode, other.Side, other.Price, other.Quantity)
        {
        }

        public OrderModel(OrderModel other)
            : this(other.TradingAccount, other.UserID, other.InstrumentCode, other.Side, other.Price, other.Quantity, other.IsEnabled, other.Status)
        {
        }

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set { this.SetProperty(ref this.isEnabled, value); }
        }

        public OrderStatus Status
        {
            get { return this.status; }
            set { this.SetProperty(ref this.status, value); }
        }
    }
}

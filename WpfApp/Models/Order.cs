using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WpfApp.Models
{
    public class Order : BindableBase
    {
        #region Fields
        private string tradingAccount;
        private Guid? userID;
        private string instrumentCode;
        private string side;
        private decimal? price;
        private long? quantity;
        #endregion

        #region Constructors
        public Order()
            : this(null, Guid.Empty, null, null, decimal.Zero, 0)
        {
        }

        public Order(string tradingAccount, Guid? userID, string instrumentCode, string side, decimal? price, long? quantity)
        {
            this.tradingAccount = tradingAccount;
            this.userID = userID;
            this.instrumentCode = instrumentCode;
            this.side = side;
            this.price = price;
            this.quantity = quantity;
        }

        public Order(Order other)
            : this(other.TradingAccount, other.UserID, other.InstrumentCode, other.Side, other.Price, other.Quantity)
        {
        }
        #endregion

        #region Properties
        [DisplayName("Trading Account")]
        [Display(Name = "Trading Account")]
        [Required(AllowEmptyStrings = false)]
        public string TradingAccount
        {
            get { return this.tradingAccount; }
            set { this.SetProperty(ref this.tradingAccount, value); }
        }

        [DisplayName("User ID")]
        [Display(Name = "User ID")]
        [Required]
        public Guid? UserID
        {
            get { return this.userID; }
            set { this.SetProperty(ref this.userID, value); }
        }

        [DisplayName("Instrument Code")]
        [Display(Name = "Instrument Code")]
        [Required(AllowEmptyStrings = false)]
        public string InstrumentCode
        {
            get { return this.instrumentCode; }
            set { this.SetProperty(ref this.instrumentCode, value); }
        }

        [DisplayName("Side")]
        [Display(Name = "Side")]
        [Required(AllowEmptyStrings = false)]
        public string Side
        {
            get { return this.side; }
            set { this.SetProperty(ref this.side, value); }
        }

        [DisplayName("Price")]
        [Display(Name = "Price")]
        [Required]
        public decimal? Price
        {
            get { return this.price; }
            set { this.SetProperty(ref this.price, value); }
        }

        [DisplayName("Quantity")]
        [Display(Name = "Quantity")]
        [Required]
        [Range(1, long.MaxValue)]
        public long? Quantity
        {
            get { return this.quantity; }
            set { this.SetProperty(ref this.quantity, value); }
        }

        public override string Error
        {
            get
            {
                if (this.ValidationErrors == null)
                {
                    return null;
                }
                return string.Join(", ", this.ValidationErrors);
            }
        }
        #endregion

        /// <summary>
        /// Parse a string (in CSV format) to Order object
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Order Parse(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return default;
            }

            try
            {
                var split = line.Split(',');
                var result = new Order(
                    split[0],
                    Guid.Parse(split[1]),
                    split[2],
                    split[3],
                    decimal.Parse(split[4]),
                    long.Parse(split[5])
                );
                return result;
            }
            catch (Exception)
            {
                return default;
            }
        }

        /// <summary>
        /// Method to write the all of the Property names (in CSV format)
        /// </summary>
        /// <returns></returns>
        public static string CSVHeaders()
        {
            return $"{nameof(TradingAccount)},{nameof(UserID)},{nameof(InstrumentCode)},{nameof(Side)},{nameof(Price)},{nameof(Quantity)}";
        }

        /// <summary>
        /// Helper method to write Order to string (in CSV format)
        /// </summary>
        /// <returns></returns>
        public string ToCSV()
        {
            return $"{this.TradingAccount},{this.UserID},{this.InstrumentCode},{this.Side},{this.Price},{this.Quantity}";
        }

        public string ForceValidate()
        {
            _ = this[nameof(this.TradingAccount)];
            _ = this[nameof(this.UserID)];
            _ = this[nameof(this.InstrumentCode)];
            _ = this[nameof(this.Side)];
            _ = this[nameof(this.Price)];
            _ = this[nameof(this.Quantity)];
            return string.Join(", ", this.ValidationErrors);
        }

    }
}

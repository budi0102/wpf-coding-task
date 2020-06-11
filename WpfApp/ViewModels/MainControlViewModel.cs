using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp.Models;
using WpfApp.Services;

namespace WpfApp.ViewModels
{
    internal class MainControlViewModel : BindableBase
    {
        private readonly IOrderService orderService;
        private string filePath;
        private ObservableCollection<OrderModel> data;

        #region Constructors
        public MainControlViewModel()
        {
            this.orderService = new OrderService(); // if DI is not available
            this.BrowseFileCommand = new DelegateCommand(p => this.BrowseFile(), p => true);
            this.LoadFileCommand = new DelegateCommand(p => this.LoadFile(), p => this.CanLoadFile());
            this.SaveFileCommand = new DelegateCommand(p => this.SaveFile(), p => this.CanSaveFile());
            this.ClearCommand = new DelegateCommand(p => this.ClearOrder(p as OrderModel), p => this.CanClearOrder(p as OrderModel));
            this.SendCommand = new DelegateCommand(p => this.Send(p as OrderModel), p => this.CanSend(p as OrderModel));
        }

        // DI Constructor
        public MainControlViewModel(IOrderService orderService)
            : this()
        {
            this.orderService = orderService;
        }
        #endregion

        public string FilePath
        {
            get { return this.filePath; }
            set { this.SetProperty(ref this.filePath, value); }
        }

        public ObservableCollection<OrderModel> Data
        {
            get { return this.data; }
            set { this.SetProperty(ref this.data, value); }
        }

        /// <summary>
        /// Browse for CSV File that contains Orders
        /// </summary>
        public ICommand BrowseFileCommand { get; }

        /// <summary>
        /// Load CSV File that contains Orders
        /// </summary>
        public ICommand LoadFileCommand { get; }

        /// <summary>
        /// Save the Orders back into the CSV file
        /// </summary>
        public ICommand SaveFileCommand { get; }

        /// <summary>
        /// Clear 1 Order Command
        /// </summary>
        public ICommand ClearCommand { get; }

        /// <summary>
        /// Exchange/ Send Order Command
        /// </summary>
        public ICommand SendCommand { get; }

        #region Command Methods
        private void BrowseFile()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                Multiselect = false,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (ofd.ShowDialog() == true)
            {
                this.FilePath = ofd.FileName;
            }
        }

        private bool CanLoadFile()
        {
            if (string.IsNullOrEmpty(this.filePath))
            {
                return false;
            }
            var fi = new FileInfo(this.filePath);
            return fi.Extension.Equals(".csv", StringComparison.OrdinalIgnoreCase) && fi.Exists;
        }

        private void LoadFile()
        {
            var orders = this.orderService.LoadFile(this.filePath);
            if (orders?.Any() ?? false)
            {
                this.Data = new ObservableCollection<OrderModel>(orders.Select(o => new OrderModel(o)));
            }
        }

        private bool CanSaveFile()
        {
            return !string.IsNullOrEmpty(this.filePath)
                // && !this.ValidationErrors.Any()
                && !(this.Data?.Any(o => o.ValidationErrors.Any()) ?? true); // can only save back if there is no validation error
        }

        private void SaveFile()
        {
            this.orderService.SaveFileAsync(this.data, this.filePath);
        }

        private bool CanClearOrder(OrderModel orderModel)
        {
            return orderModel != null;
        }

        private void ClearOrder(OrderModel orderModel)
        {
            if (orderModel == null)
            {
                return;
            }

            this.Data.Remove(orderModel);
        }

        private bool CanSend(OrderModel orderModel)
        {
            return orderModel != null
                && !orderModel.ValidationErrors.Any(); // can only save back if there is no validation error
        }

        /// <summary>
        /// Simulate sending the order
        /// 1. edit the order status to disabled and status=sending
        /// 2. once we received the feedback from the service, update the order status back.
        /// </summary>
        /// <param name="orderModel"></param>
        private async void Send(OrderModel orderModel)
        {
            if (orderModel == null)
            {
                return;
            }

            orderModel.IsEnabled = false;
            orderModel.Status = OrderStatus.Sending;

            await Task.Run(() =>
            {
                var status = this.orderService.SendOrder(orderModel);
                orderModel.Status = status;
                orderModel.IsEnabled = true;
            });
        }
        #endregion
    }
}

using CakeShopApp.Model;
using CakeShopApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CakeShopApp.ViewModels
{
    class InvoiceListUCViewModel : BaseViewModel
    {
        #region variables

        #endregion

        #region properties

        // SHOW LIST AND SELECTED ITEM

        private int _totalInvoice;
        public int TotalInvoice { get => _totalInvoice; set { _totalInvoice = value; OnPropertyChanged(); } }

        private int _checkedInvoice;
        public int CheckedInvoice { get => _checkedInvoice; set { _checkedInvoice = value; OnPropertyChanged(); } }

        private int _unCheckedInvoice;
        public int UnCheckedInvoice { get => _unCheckedInvoice; set { _unCheckedInvoice = value; OnPropertyChanged(); } }

        private AsyncObservableCollection<dynamic> _invoices;
        public AsyncObservableCollection<dynamic> Invoices { get => _invoices; set { _invoices = value; OnPropertyChanged(); } }

        private Root _selectedInvoice;
        public Root SelectedInvoice
        {
            get => _selectedInvoice; set
            {
                _selectedInvoice = value;
                
                OnPropertyChanged();
            }
        }

        
        private bool _isOpenDialog;
        public bool IsOpenDialog
        {
            get => _isOpenDialog;
            set
            {
                _isOpenDialog = value;
                if (value == true)
                {
                    
                }
                else
                {
                    
                }
                OnPropertyChanged();
            }
        }

        #endregion

        #region commands
        public ICommand CheckShipCommand { get; set; }

        #endregion

        public InvoiceListUCViewModel()
        {
            TotalInvoice = 0;
            CheckedInvoice = 0;
            UnCheckedInvoice = 0;
            Invoices = new AsyncObservableCollection<dynamic>();
            var a = DataProvider.Ins.DB.Invoices.OrderByDescending(x => x.CreatedDate).ToList();
            foreach (var invoice in DataProvider.Ins.DB.Invoices.OrderByDescending(x => x.CreatedDate))
            {
                string date;
                if (invoice.PaymentMethod == 1)
                {
                    date = invoice.CreatedDate.ToString().Split(' ')[0];
                }
                else
                {
                    date = invoice.DeliveryPayment.ShippingDate.ToString().Split(' ')[0];
                }
                Invoices.Add(new
                {
                    Id = invoice.Id,
                    CustomerName = invoice.Name,
                    Date = date,
                    Status = invoice.Status,
                });
                TotalInvoice++;
                if (invoice.Status == "Đã thanh toán")
                {
                    CheckedInvoice++;
                }
                else
                {
                    UnCheckedInvoice++;
                }
            }


            CheckShipCommand = new RelayCommand<dynamic>((param) => { return true; }, (param) =>
            {
                int index = Invoices.IndexOf(param);
                Invoice invoice = DataProvider.Ins.DB.Invoices.Find(param.Id);
                invoice.Status = "Đã thanh toán";
                DataProvider.Ins.DB.SaveChanges();
                Invoices.RemoveAt(index);
                Invoices.Insert(index, new {
                    Id = invoice.Id,
                    CustomerName = invoice.Name,
                    Date = invoice.DeliveryPayment.ShippingDate.ToString().Split(' ')[0],
                    Status = invoice.Status,
                });
                CheckedInvoice++;
                UnCheckedInvoice--;
            });
        }
    }
}

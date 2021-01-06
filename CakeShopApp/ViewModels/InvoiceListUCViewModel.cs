using CakeShopApp.Model;
using CakeShopApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CakeShopApp.ViewModels
{
    class InvoiceListUCViewModel : BaseViewModel
    {
        #region variables

        #endregion

        #region properties

        private string _search;
        public string Search { get => _search; set { _search = value; CallSearch(); OnPropertyChanged(); } }

        // SHOW LIST AND SELECTED ITEM

        private int _totalInvoice;
        public int TotalInvoice { get => _totalInvoice; set { _totalInvoice = value; OnPropertyChanged(); } }

        private int _checkedInvoice;
        public int CheckedInvoice { get => _checkedInvoice; set { _checkedInvoice = value; OnPropertyChanged(); } }

        private int _unCheckedInvoice;
        public int UnCheckedInvoice { get => _unCheckedInvoice; set { _unCheckedInvoice = value; OnPropertyChanged(); } }

        private AsyncObservableCollection<dynamic> _invoices;
        public AsyncObservableCollection<dynamic> Invoices { get => _invoices; set { _invoices = value; OnPropertyChanged(); } }

        private dynamic _selectedInvoice;
        public dynamic SelectedInvoice
        {
            get => _selectedInvoice; set
            {
                _selectedInvoice = value;
                IsOpenDetailDialog = true;
                OnPropertyChanged();
            }
        }

        //Show Detail

        private bool _isOpenDetailDialog;
        public bool IsOpenDetailDialog
        {
            get => _isOpenDetailDialog; set
            {
                _isOpenDetailDialog = value;
                if (value == true)
                {
                    Invoice invoice = DataProvider.Ins.DB.Invoices.Find(SelectedInvoice.Id);
                    DetailId = invoice.Id;
                    DetailDateCreate = invoice.CreatedDate.ToString().Split(' ')[0];
                    DetailCash = (invoice.PaymentMethod == 1) ? invoice.DirectPayment.Cash.ToString() : null;
                    DetailChange = (invoice.PaymentMethod == 1) ? invoice.DirectPayment.Change.ToString() : null;
                    DirectPaymentVisibility = (invoice.PaymentMethod == 1) ? Visibility.Visible : Visibility.Hidden;
                    DetailName = invoice.Name;
                    DetailPhone = invoice.Phone;
                    DetailAddress = (invoice.PaymentMethod == 2) ? invoice.DeliveryPayment.Address : null;
                    DetailPostPaid = (invoice.PaymentMethod == 2) ? invoice.DeliveryPayment.PostPaid.ToString() : null;
                    DetailPrePaid = (invoice.PaymentMethod == 2) ? invoice.DeliveryPayment.PrePaid.ToString() : null;
                    DetailDateShip = (invoice.PaymentMethod == 2) ? invoice.DeliveryPayment.ShippingDate.ToString().Split(' ')[0] : null;
                    DeliveryPaymentVisibility = (invoice.PaymentMethod == 2) ? Visibility.Visible : Visibility.Hidden;
                    DetailTotal = invoice.Total.ToString();
                    DetailInvoices = new AsyncObservableCollection<dynamic>();
                    foreach (var detail in invoice.InvoiceDetails)
                    {
                        Product product = DataProvider.Ins.DB.Products.Find(detail.ProductId);
                        DetailInvoices.Add(new { 
                            Name = product.Name,
                            Quantity = detail.Amount + detail.GiftAmount,
                            Price = product.SellPrice.ToString(),
                            Total = (detail.Amount * product.SellPrice * (100 - detail.Discount) / 100).ToString(),
                        });
                    }
                }
                else
                {
                    DetailId = 0;
                    DetailAddress = null;
                    DetailCash = null;
                    DetailChange = null;
                    DetailName = null;
                    DetailInvoices = null;
                    DetailPhone = null;
                    DetailPostPaid = null;
                    DetailPrePaid = null;
                    DetailTotal = null;
                }
                OnPropertyChanged();
            }
        }

        private int _detailId;
        public int DetailId { get => _detailId; set { _detailId = value; OnPropertyChanged(); } }

        private string _detailName;
        public string DetailName { get => _detailName; set { _detailName = value; OnPropertyChanged(); } }

        private string _detailPhone;
        public string DetailPhone { get => _detailPhone; set { _detailPhone = value; OnPropertyChanged(); } }

        private AsyncObservableCollection<dynamic> _detailInvoices;
        public AsyncObservableCollection<dynamic> DetailInvoices { get => _detailInvoices; set { _detailInvoices = value; OnPropertyChanged(); } }

        private string _detailCash;
        public string DetailCash { get => _detailCash; set { _detailCash = value; OnPropertyChanged(); } }

        private string _detailChange;
        public string DetailChange { get => _detailChange; set { _detailChange = value; OnPropertyChanged(); } }

        private string _detailDateShip;
        public string DetailDateShip { get => _detailDateShip; set { _detailDateShip = value; OnPropertyChanged(); } }
        
        private string _detailDateCreate;
        public string DetailDateCreate { get => _detailDateCreate; set { _detailDateCreate = value; OnPropertyChanged(); } }

        private string _detailAddress;
        public string DetailAddress { get => _detailAddress; set { _detailAddress = value; OnPropertyChanged(); } }

        private string _detailPrePaid;
        public string DetailPrePaid { get => _detailPrePaid; set { _detailPrePaid = value; OnPropertyChanged(); } }

        private string _detailPostPaid;
        public string DetailPostPaid { get => _detailPostPaid; set { _detailPostPaid = value; OnPropertyChanged(); } }

        private string _detailTotal;
        public string DetailTotal { get => _detailTotal; set { _detailTotal = value; OnPropertyChanged(); } }

        private Visibility _directPaymentVisibility;
        public Visibility DirectPaymentVisibility { get => _directPaymentVisibility; set { _directPaymentVisibility = value; OnPropertyChanged(); } }

        private Visibility _deliveryPaymentVisibility;
        public Visibility DeliveryPaymentVisibility { get => _deliveryPaymentVisibility; set { _deliveryPaymentVisibility = value; OnPropertyChanged(); } }

        #endregion

        #region commands
        public ICommand CheckShipCommand { get; set; }

        #endregion

        public InvoiceListUCViewModel()
        {
            TotalInvoice = 0;
            CheckedInvoice = 0;
            UnCheckedInvoice = 0;
            LoadInvoices();
            

            CheckShipCommand = new RelayCommand<dynamic>((param) => { return true; }, (param) =>
            {
                MessageBoxResult result = MessageBox.Show("Đơn hàng đã được thanh toán ?", "CẢNH BÁO", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    int index = Invoices.IndexOf(param);
                    Invoice invoice = DataProvider.Ins.DB.Invoices.Find(param.Id);
                    invoice.Status = "Đã thanh toán";
                    DataProvider.Ins.DB.SaveChanges();
                    Invoices.RemoveAt(index);
                    Invoices.Insert(index, new
                    {
                        Id = invoice.Id,
                        Name = invoice.Name,
                        Date = invoice.DeliveryPayment.ShippingDate.ToString().Split(' ')[0],
                        Status = invoice.Status,
                    });
                    CheckedInvoice++;
                    UnCheckedInvoice--;
                }
                    
            });
        }
        private void LoadInvoices()
        {
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
                    Name = invoice.Name,
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
        }
        private void CallSearch()
        {
            LoadInvoices();
            Invoices = SearchByName(Search, Invoices);
        }
    }
}

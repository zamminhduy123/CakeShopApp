using CakeShopApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CakeShopApp.Models;
using System.Windows.Input;
using System.Data.Entity.Validation;
using System.Windows.Controls;
using System.Globalization;

namespace CakeShopApp.ViewModels
{
    class HomeUCViewModel : BaseViewModel
    {
        #region variables
        private static HomeUCViewModel _instance = null;
        private static object m_lock = new object();
        private List<string> _listSort = new List<string>() { 
            "Bán chạy",
            "Hàng mới",
            "A-Z",
            "Z-A",
            "Giá tăng dần",
            "Giá giảm dần",
        };
        #endregion
        #region valid value
        private bool _isValidName;
        public bool IsValidName { get => _isValidName; set { _isValidName = value; OnPropertyChanged(); } }
        private bool _isValidPhone;
        public bool IsValidPhone { get => _isValidPhone; set { _isValidPhone = value; OnPropertyChanged(); } }
        private bool _isValidCheckoutCash;
        public bool IsValidCheckoutCash { get => _isValidCheckoutCash; set { _isValidCheckoutCash = value; OnPropertyChanged(); } }
        private bool _isValidCheckoutOff;
        public bool IsValidCheckoutOff { get => _isValidCheckoutOff; set { _isValidCheckoutOff = value; OnPropertyChanged(); } }
        private bool _isValidCheckoutAdress;
        public bool IsValidCheckoutAdress { get => _isValidCheckoutAdress; set { _isValidCheckoutAdress = value; OnPropertyChanged(); } }
        private bool _isValidCheckoutPrePaid;
        public bool IsValidCheckoutPrePaid { get => _isValidCheckoutPrePaid; set { _isValidCheckoutPrePaid = value; OnPropertyChanged(); } }
        private bool _isValidCheckoutDate;
        public bool IsValidCheckoutDate { get => _isValidCheckoutDate; set { _isValidCheckoutDate = value; OnPropertyChanged(); } }
        #endregion
        #region enable button
        private bool _isEnabledSecondCheckoutButton;
        public bool IsEnabledSecondCheckoutButton { get => _isEnabledSecondCheckoutButton; set { _isEnabledSecondCheckoutButton = value; OnPropertyChanged(); } }

        private bool _isEnabledFirstCheckoutButton;
        public bool IsEnabledFirstCheckoutButton { get => _isEnabledFirstCheckoutButton; set { _isEnabledFirstCheckoutButton = value; OnPropertyChanged(); } }

        #endregion
        #region properties

        private string _search;
        public string Search { get => _search; set { _search = value; CallSearch(); OnPropertyChanged(); } }
        // Sort & Sorting

        private AsyncObservableCollection<dynamic> _categories;
        public AsyncObservableCollection<dynamic> Categories { get => _categories; set { _categories = value; OnPropertyChanged(); } }

        private dynamic _selectedCategory;
        public dynamic SelectedCategory
        {
            get => _selectedCategory; set
            {
                _selectedCategory = value;
                if (SelectedCategory != null && SelectedSort != null)
                {
                    CallSearch();
                }
                SelectedProduct = null;
                OnPropertyChanged();
            }
        }

        private AsyncObservableCollection<string> _sorts;
        public AsyncObservableCollection<string> Sorts { get => _sorts; set { _sorts = value; OnPropertyChanged(); } }

        private string _selectedSort;
        public string SelectedSort { get => _selectedSort; set { _selectedSort = value;
                CallSearch();
                OnPropertyChanged(); } }

        // make invoice

        private AsyncObservableCollection<dynamic> _products;
        public AsyncObservableCollection<dynamic> Products { get => _products; set { _products = value; OnPropertyChanged(); } }

        private dynamic _selectedProduct;
        public dynamic SelectedProduct { get => _selectedProduct; set { _selectedProduct = value; OnPropertyChanged(); } }

        private AsyncObservableCollection<DetailInList> _invoiceDetails;
        public AsyncObservableCollection<DetailInList> InvoiceDetails { get => _invoiceDetails; set { _invoiceDetails = value; OnPropertyChanged(); } }

        private string _detailInListTotalPrice;
        public string DetailInListTotalPrice { get => _detailInListTotalPrice; set { _detailInListTotalPrice = value; OnPropertyChanged(); } }

        //Check out

        private int _checkOutId;
        public int CheckOutId { get => _checkOutId; set { _checkOutId = value; OnPropertyChanged(); } }

        private bool _isOpenCheckOutDialog;
        public bool IsOpenCheckOutDialog
        {
            get => _isOpenCheckOutDialog; set
            {
                _isOpenCheckOutDialog = value;
                if (value == true)
                {
                    Invoice newinvoice = new Invoice() { Name = "", Phone = "", PaymentMethod = 1, CreatedDate = DateTime.Now, Status = "", Total = 0, };
                    DataProvider.Ins.DB.Invoices.Add(newinvoice);
                    DataProvider.Ins.DB.SaveChanges();
                    CheckOutId = newinvoice.Id;
                    CheckOutDetails = new AsyncObservableCollection<dynamic>();
                    CheckOutDateShip = DateTime.Now;
                    foreach (var invoicedetail in InvoiceDetails)
                    {
                        CheckOutDetails.Add(new { 
                            Name = invoicedetail.ProductName,
                            Quantity = invoicedetail.Amount + invoicedetail.GiftAmount,
                            Price = invoicedetail.ProductPrice,
                            Total = invoicedetail.SummaryPrice,
                        });
                    }
                    CheckOutTotal = DetailInListTotalPrice;
                    // valid
                    IsValidName = false;
                    IsValidPhone = false;
                    IsValidCheckoutCash = false;
                    IsValidCheckoutAdress = false;
                    IsValidCheckoutPrePaid = false;
                    IsValidCheckoutOff = true;
                    IsValidCheckoutDate = true;
                }
                else
                {
                    CheckOutId = 0;
                    CheckOutAddress = null;
                    CheckOutCash = null;
                    CheckOutChange = null;
                    CheckOutCustomerName = null;
                    CheckOutDetails = null;
                    CheckOutOff = 0;
                    CheckOutPhone = null;
                    CheckOutPostPaid = null;
                    CheckOutPrePaid = null;
                    CheckOutTotal = null;
                }
                CheckOutCustomerName = null;
                CheckOutPhone = null;
                CheckOutCash = null;
                CheckOutOff = 0;
                IsEnabledSecondCheckoutButton = false;
                OnPropertyChanged();
            }
        }

        private string _checkOutCustomerName;
        public string CheckOutCustomerName 
        { 
            get => _checkOutCustomerName; 
            set 
            { 
                _checkOutCustomerName = value;
                IsEnabledSecondCheckoutButton = IsCheckOut();
                if (value != null)
                {
                    IsValidName = true;
                }
                OnPropertyChanged(); 
            } 
        }

        private string _checkOutPhone;
        public string CheckOutPhone 
        { 
            get => _checkOutPhone;
            set 
            { 
                _checkOutPhone = value;
                IsEnabledSecondCheckoutButton = IsCheckOut();
                if (value != null)
                {
                    IsValidPhone = true;
                }
                OnPropertyChanged(); 
            } 
        }

        private AsyncObservableCollection<dynamic> _checkOutDetails;
        public AsyncObservableCollection<dynamic> CheckOutDetails { get => _checkOutDetails; set { _checkOutDetails = value; OnPropertyChanged(); } }

        private string _checkOutCash;
        public string CheckOutCash { get => _checkOutCash; set { _checkOutCash = value;
                if (CheckOutCash != null && IsValidName == true && IsValidPhone == true && IsValidCheckoutOff == true)
                {
                    IsEnabledSecondCheckoutButton = IsCheckOut();
                    CheckOutChange = (int.Parse(CheckOutCash) - int.Parse(CheckOutTotal)).ToString();
                }
                if (value != null)
                {
                    IsValidCheckoutCash = true;
                }
                OnPropertyChanged(); } }

        private string _checkOutChange;
        public string CheckOutChange { get => _checkOutChange; set { _checkOutChange = value; OnPropertyChanged(); } }

        private DateTime _checkOutDateShip;
        public DateTime CheckOutDateShip { get => _checkOutDateShip; set { _checkOutDateShip = value;
                IsEnabledSecondCheckoutButton = IsCheckOut();
                OnPropertyChanged(); } }

        private string _checkOutAddress;
        public string CheckOutAddress { get => _checkOutAddress; set { _checkOutAddress = value;
                if (value != null)
                {
                    IsValidCheckoutAdress = true;
                }
                IsEnabledSecondCheckoutButton = IsCheckOut();
                OnPropertyChanged(); }
        }

        private string _checkOutPrePaid;
        public string CheckOutPrePaid { get => _checkOutPrePaid; set { _checkOutPrePaid = value;
                if (CheckOutPrePaid != null)
                {
                    CheckOutPostPaid = (int.Parse(CheckOutTotal) - int.Parse(CheckOutPrePaid)).ToString();
                    IsValidCheckoutPrePaid = true;
                }
                IsEnabledSecondCheckoutButton = IsCheckOut();
                OnPropertyChanged(); }
        }

        private string _checkOutPostPaid;
        public string CheckOutPostPaid { get => _checkOutPostPaid; set { _checkOutPostPaid = value; OnPropertyChanged(); } }

        private bool _isDelivery;
        public bool IsDelivery { get => _isDelivery; set { _isDelivery = value;
                IsEnabledSecondCheckoutButton = IsCheckOut();
                OnPropertyChanged(); } }

        private int _checkOutOff;
        public int CheckOutOff { get => _checkOutOff; set { _checkOutOff = value;
                if (value != null)
                {
                    IsValidCheckoutOff = true;
                }
                if (CheckOutOff != null && IsOpenCheckOutDialog == true && IsValidName == true && IsValidPhone == true && IsValidCheckoutCash == true)
                {
                    IsEnabledSecondCheckoutButton = IsCheckOut();
                    for (int i = 0; i < InvoiceDetails.Count(); i++)
                    {
                        var detail = InvoiceDetails.ElementAt(i);
                        if (detail.Discount < CheckOutOff)
                        {
                            CheckOutDetails.RemoveAt(i);
                            CheckOutDetails.Insert( i, new
                            {
                                Name = detail.ProductName,
                                Quantity = detail.Amount + detail.GiftAmount,
                                Price = detail.ProductPrice,
                                Total = (int.Parse(detail.ProductPrice) * detail.Amount * (100 - CheckOutOff) / 100).ToString(),
                            });
                        }
                    }
                    CheckOutTotal = "0";
                    foreach (var detail in CheckOutDetails)
                    {
                        CheckOutTotal = (int.Parse(CheckOutTotal) + int.Parse(detail.Total)).ToString();
                    }
                    CheckOutCash = CheckOutCash;
                    CheckOutPrePaid = CheckOutPrePaid;
                }
                OnPropertyChanged(); } }

        private string _checkOutTotal;
        public string CheckOutTotal { get => _checkOutTotal; set { _checkOutTotal = value; OnPropertyChanged(); } }

        #endregion

        #region commands
        public ICommand DeleteDetailInListCommand { get; set; }
        public ICommand LoadPrice { get; set; }
        public ICommand AddInvoiceCommand { get; set; }
        public ICommand AddToCartCommand { get; set; }
        public ICommand ChangeCategoryCommand { get; set; }
        //disable command
        public ICommand DisableName { get; set; }
        public ICommand DisablePhone { get; set; }
        public ICommand DisableCheckoutCash { get; set; }
        public ICommand DisableCheckoutOff { get; set; }
        public ICommand DisableSecondCheckout { get; set; }
        public ICommand DisableCheckoutAdress { get; set; }
        public ICommand DisableCheckoutPrePaid { get; set; }
        public ICommand DisableCheckoutDate { get; set; }

        #endregion

        public static HomeUCViewModel GetInstance()
        {
            // DoubleLock
            if (_instance == null)
            {
                lock (m_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new HomeUCViewModel();
                    }
                }
            }
            return _instance;
        }
        public HomeUCViewModel()
        {
            // khởi tạo dữ liệu
            InvoiceDetails = new AsyncObservableCollection<DetailInList>();

            DetailInListTotalPrice = "0";

            Categories = new AsyncObservableCollection<dynamic>();
            Categories.Add(new
            {
                Id = 0,
                Name = "Tất cả",
                PickIcon = "ArrowExpandAll"
            });
            foreach (var category in DataProvider.Ins.DB.Categories)
            {
                Categories.Add(new
                {
                    Id = category.Id,
                    Name = category.Name,
                    PickIcon = "FoodCroissant"
                });
            }
            SelectedCategory = Categories.ElementAt(0);

            Sorts = new AsyncObservableCollection<string>();
            foreach (var sort in _listSort)
            {
                Sorts.Add(sort);
            }
            SelectedSort = Sorts.ElementAt(0);

            //Products = new AsyncObservableCollection<dynamic>();
            //foreach (var product in DataProvider.Ins.DB.Products.Where(x => x.IsHidden == 0))
            //{
            //    Products.Add(new
            //    {
            //        Id = product.Id,
            //        Name = product.Name,
            //        Thumbnail = (product.Photos.Count == 0) ? null : product.Photos.ToList()[0].ImageBytes,
            //        Price = product.SellPrice.ToString(),
            //    });
            //}

            // Commands
            DeleteDetailInListCommand = new RelayCommand<DetailInList>((param) => { return true; }, (param) =>
            {
                DetailInListTotalPrice = (int.Parse(DetailInListTotalPrice) - int.Parse(param.SummaryPrice)).ToString();
                InvoiceDetails.Remove(param);
                if (InvoiceDetails.Count() == 0)
                {
                    IsEnabledFirstCheckoutButton = false;
                }
                CallSearch();
            });
            LoadPrice = new RelayCommand<DetailInList>((param) => { return true; }, (param) =>
            {
                DetailInListTotalPrice = "0";
                param.SummaryPrice = (int.Parse(param.ProductPrice) * param.Amount * (100 - param.Discount) / 100).ToString();
                foreach (var invoicedetail in InvoiceDetails)
                {
                    DetailInListTotalPrice = (int.Parse(DetailInListTotalPrice) + int.Parse(invoicedetail.SummaryPrice)).ToString();
                }
            });
            AddInvoiceCommand = new RelayCommand<string>((param) => { return true; }, (param) =>
            {
                if (bool.Parse(param) == true)
                {
                    Invoice newinvoice = DataProvider.Ins.DB.Invoices.Find(CheckOutId);
                    newinvoice.Name = CheckOutCustomerName;
                    newinvoice.Phone = (CheckOutPhone == null) ? "" : CheckOutPhone;
                    newinvoice.CreatedDate = DateTime.Now;
                    newinvoice.PaymentMethod = (IsDelivery == true) ? 2 : 1;
                    newinvoice.Status = (IsDelivery == true) ? "Chờ giao hàng" : "Đã thanh toán";
                    newinvoice.Total = int.Parse(CheckOutTotal);
                    if (newinvoice.PaymentMethod == 1)
                    {
                        DirectPayment newdirectpayment = new DirectPayment() { InvoiceId = newinvoice.Id, Cash = int.Parse(CheckOutCash), Change = int.Parse(CheckOutChange), };
                        DataProvider.Ins.DB.DirectPayments.Add(newdirectpayment);
                    }
                    else
                    {
                        DeliveryPayment newdeliverypayment = new DeliveryPayment() { InvoiceId = newinvoice.Id, Address = CheckOutAddress, ShippingDate = CheckOutDateShip, PrePaid = int.Parse(CheckOutPrePaid), PostPaid = int.Parse(CheckOutPostPaid), };
                        DataProvider.Ins.DB.DeliveryPayments.Add(newdeliverypayment);
                    }
                    foreach (var invoicedetail in InvoiceDetails)
                    {
                        newinvoice.InvoiceDetails.Add(new InvoiceDetail
                        {
                            InvoiceId = CheckOutId,
                            ProductId = invoicedetail.ProductId,
                            Amount = invoicedetail.Amount,
                            Discount = (invoicedetail.Discount > CheckOutOff) ? invoicedetail.Discount : CheckOutOff,
                            GiftAmount = invoicedetail.GiftAmount,
                        });
                        DataProvider.Ins.DB.Products.Find(invoicedetail.ProductId).InStockAmount = DataProvider.Ins.DB.Products.Find(invoicedetail.ProductId).InStockAmount - (invoicedetail.Amount + invoicedetail.GiftAmount);
                    }
                    DataProvider.Ins.DB.SaveChanges();
                    InvoiceDetails = new AsyncObservableCollection<DetailInList>();
                    DetailInListTotalPrice = "0";
                    CallSearch();
                }
                else
                {
                    DataProvider.Ins.DB.Invoices.Remove(DataProvider.Ins.DB.Invoices.Find(CheckOutId));
                    DataProvider.Ins.DB.SaveChanges();
                }
                IsOpenCheckOutDialog = false;
            });
            AddToCartCommand = new RelayCommand<dynamic>((param) => { return true; }, (param) =>
            {
                InvoiceDetails.Add(new DetailInList
                {
                    ProductId = param.Id,
                    ProductName = param.Name,
                    ProductThumbnail = param.Thumbnail,
                    ProductPrice = param.Price,
                    Amount = 1,
                    Discount = 0,
                    GiftAmount = 0,
                    SummaryPrice = param.Price,
                });
                DetailInListTotalPrice = (int.Parse(DetailInListTotalPrice) + int.Parse(param.Price)).ToString();
                Products.Remove(param);
                SelectedProduct = null;
            });
            ChangeCategoryCommand = new RelayCommand<dynamic>((param) => { return true; }, (param) => {
                SelectedCategory = param;
                CallSearch();
                SelectedProduct = null;
            });
            DisableName = new RelayCommand<dynamic>((param) => { return true; }, (param) => {
                IsValidName = false;
                IsEnabledSecondCheckoutButton = false;
            });
            DisablePhone = new RelayCommand<dynamic>((param) => { return true; }, (param) => {
                IsValidPhone = false;
                IsEnabledSecondCheckoutButton = false;
            });
            DisableCheckoutCash = new RelayCommand<dynamic>((param) => { return true; }, (param) => {
                IsValidCheckoutCash = false;
                IsEnabledSecondCheckoutButton = false;
            });
            DisableCheckoutOff = new RelayCommand<dynamic>((param) => { return true; }, (param) => {
                IsValidCheckoutOff = false;
                IsEnabledSecondCheckoutButton = false;
            });
            DisableSecondCheckout = new RelayCommand<dynamic>((param) => { return true; }, (param) => {
                IsEnabledSecondCheckoutButton = false;
            });
            DisableCheckoutAdress = new RelayCommand<dynamic>((param) => { return true; }, (param) => {
                IsValidCheckoutAdress = false;
                IsEnabledSecondCheckoutButton = false;
            });
            DisableCheckoutPrePaid = new RelayCommand<dynamic>((param) => { return true; }, (param) => {
                IsValidCheckoutPrePaid = false;
                IsEnabledSecondCheckoutButton = false;
            });
            DisableCheckoutDate = new RelayCommand<dynamic>((param) => { return true; }, (param) => {
                IsValidCheckoutDate = false;
                IsEnabledSecondCheckoutButton = false;
            });
        }
        private void LoadProducts()
        {

            Products = new AsyncObservableCollection<dynamic>();
            if (SelectedCategory.Id == 0)
            {
                switch (_listSort.IndexOf(SelectedSort))
                {
                    case 0:
                        Products = new AsyncObservableCollection<dynamic>();
                        foreach (var product in DataProvider.Ins.DB.Products.Where(x => x.IsHidden == 0).OrderByDescending(x => x.ImportAmount - x.InStockAmount))
                        {
                            Products.Add(new
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Thumbnail = (product.Photos.Count == 0) ? null : product.Photos.ToList()[0].ImageBytes,
                                Price = product.SellPrice.ToString(),
                            });
                        }
                        break;
                    case 1:
                        Products = new AsyncObservableCollection<dynamic>();
                        foreach (var product in DataProvider.Ins.DB.Products.Where(x => x.IsHidden == 0).OrderByDescending(x => x.ImportDate))
                        {
                            Products.Add(new
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Thumbnail = (product.Photos.Count == 0) ? null : product.Photos.ToList()[0].ImageBytes,
                                Price = product.SellPrice.ToString(),
                            });
                        }
                        break;
                    case 2:
                        Products = new AsyncObservableCollection<dynamic>();
                        foreach (var product in DataProvider.Ins.DB.Products.Where(x => x.IsHidden == 0).OrderBy(x => x.Name))
                        {
                            Products.Add(new
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Thumbnail = (product.Photos.Count == 0) ? null : product.Photos.ToList()[0].ImageBytes,
                                Price = product.SellPrice.ToString(),
                            });
                        }
                        break;
                    case 3:
                        Products = new AsyncObservableCollection<dynamic>();
                        foreach (var product in DataProvider.Ins.DB.Products.Where(x => x.IsHidden == 0).OrderByDescending(x => x.Name))
                        {
                            Products.Add(new
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Thumbnail = (product.Photos.Count == 0) ? null : product.Photos.ToList()[0].ImageBytes,
                                Price = product.SellPrice.ToString(),
                            });
                        }
                        break;
                    case 4:
                        Products = new AsyncObservableCollection<dynamic>();
                        foreach (var product in DataProvider.Ins.DB.Products.Where(x => x.IsHidden == 0).OrderBy(x => x.SellPrice))
                        {
                            Products.Add(new
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Thumbnail = (product.Photos.Count == 0) ? null : product.Photos.ToList()[0].ImageBytes,
                                Price = product.SellPrice.ToString(),
                            });
                        }
                        break;
                    case 5:
                        Products = new AsyncObservableCollection<dynamic>();
                        foreach (var product in DataProvider.Ins.DB.Products.Where(x => x.IsHidden == 0).OrderByDescending(x => x.SellPrice))
                        {
                            Products.Add(new
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Thumbnail = (product.Photos.Count == 0) ? null : product.Photos.ToList()[0].ImageBytes,
                                Price = product.SellPrice.ToString(),
                            });
                        }
                        break;
                }
            }
            else
            {
                int categoryid = SelectedCategory.Id;
                switch (_listSort.IndexOf(SelectedSort))
                {
                    case 0:
                        Products = new AsyncObservableCollection<dynamic>();
                        foreach (var product in DataProvider.Ins.DB.Products.Where(x => x.IsHidden == 0 && x.CategoryId == categoryid).OrderByDescending(x => x.ImportAmount - x.InStockAmount))
                        {
                            Products.Add(new
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Thumbnail = (product.Photos.Count == 0) ? null : product.Photos.ToList()[0].ImageBytes,
                                Price = product.SellPrice.ToString(),
                            });
                        }
                        break;
                    case 1:
                        Products = new AsyncObservableCollection<dynamic>();
                        foreach (var product in DataProvider.Ins.DB.Products.Where(x => x.IsHidden == 0 && x.CategoryId == categoryid).OrderByDescending(x => x.ImportDate))
                        {
                            Products.Add(new
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Thumbnail = (product.Photos.Count == 0) ? null : product.Photos.ToList()[0].ImageBytes,
                                Price = product.SellPrice.ToString(),
                            });
                        }
                        break;
                    case 2:
                        Products = new AsyncObservableCollection<dynamic>();
                        foreach (var product in DataProvider.Ins.DB.Products.Where(x => x.IsHidden == 0 && x.CategoryId == categoryid).OrderBy(x => x.Name))
                        {
                            Products.Add(new
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Thumbnail = (product.Photos.Count == 0) ? null : product.Photos.ToList()[0].ImageBytes,
                                Price = product.SellPrice.ToString(),
                            });
                        }
                        break;
                    case 3:
                        Products = new AsyncObservableCollection<dynamic>();
                        foreach (var product in DataProvider.Ins.DB.Products.Where(x => x.IsHidden == 0 && x.CategoryId == categoryid).OrderByDescending(x => x.Name))
                        {
                            Products.Add(new
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Thumbnail = (product.Photos.Count == 0) ? null : product.Photos.ToList()[0].ImageBytes,
                                Price = product.SellPrice.ToString(),
                            });
                        }
                        break;
                    case 4:
                        Products = new AsyncObservableCollection<dynamic>();
                        foreach (var product in DataProvider.Ins.DB.Products.Where(x => x.IsHidden == 0 && x.CategoryId == categoryid).OrderBy(x => x.SellPrice))
                        {
                            Products.Add(new
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Thumbnail = (product.Photos.Count == 0) ? null : product.Photos.ToList()[0].ImageBytes,
                                Price = product.SellPrice.ToString(),
                            });
                        }
                        break;
                    case 5:
                        Products = new AsyncObservableCollection<dynamic>();
                        foreach (var product in DataProvider.Ins.DB.Products.Where(x => x.IsHidden == 0 && x.CategoryId == categoryid).OrderByDescending(x => x.SellPrice))
                        {
                            Products.Add(new
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Thumbnail = (product.Photos.Count == 0) ? null : product.Photos.ToList()[0].ImageBytes,
                                Price = product.SellPrice.ToString(),
                            });
                        }
                        break;
                }
            }
            var tmp = Products.ToList();
            foreach (var product in tmp)
            {
                int prodid = product.Id;
                if (InvoiceDetails.Where(x => x.ProductId == prodid).Count() > 0)
                {
                    Products.Remove(product);
                }
            }
        }
        private bool IsCheckOut()
        {
            if (IsDelivery == true)
            {
                return (IsValidName && IsValidPhone && IsValidCheckoutOff && IsValidCheckoutPrePaid && IsValidCheckoutAdress && IsValidCheckoutDate);
            }
            else
            {
                return (IsValidName && IsValidPhone && IsValidCheckoutOff && IsValidCheckoutCash);
            }
        }
        private void CallSearch()
        {
            LoadProducts();
            Products = SearchByName(Search, Products);
        }
    }
    // tạo class hỗ trợ trình bày dữ liệu lên view
    public class DetailInList : BaseViewModel
    {
        #region valid value
        private bool _isValidAmount;
        public bool IsValidAmount { get => _isValidAmount; set { _isValidAmount = value; OnPropertyChanged(); } }
        private bool _isValidDiscount;
        public bool IsValidDiscount { get => _isValidDiscount; set { _isValidDiscount = value; OnPropertyChanged(); } }
        private bool _isValidGiftAmount;
        public bool IsValidGiftAmount { get => _isValidGiftAmount; set { _isValidGiftAmount = value; OnPropertyChanged(); } }

        #endregion
        private bool _isEnabledCheckoutButton;
        public bool IsEnabledCheckoutButton 
        { 
            get => _isEnabledCheckoutButton; 
            set 
            { 
                _isEnabledCheckoutButton = value; 
                HomeUCViewModel.GetInstance().IsEnabledFirstCheckoutButton = IsEnabledCheckoutButton; 
                OnPropertyChanged(); 
            } 
        }
        #region properties
        private int _productId;
        public int ProductId { get => _productId; set { _productId = value; OnPropertyChanged(); } }

        private string _productName;
        public string ProductName { get => _productName; set { _productName = value; OnPropertyChanged(); } }

        private byte[] _productThumbnail;
        public byte[] ProductThumbnail { get => _productThumbnail; set { _productThumbnail = value; OnPropertyChanged(); } }

        private string _productPrice;
        public string ProductPrice { get => _productPrice; set { _productPrice = value; OnPropertyChanged(); } }

        private int _amount;
        public int Amount 
        { 
            get => _amount; 
            set 
            { 
                _amount = value;
                if (IsValidDiscount == true && IsValidGiftAmount == true)
                {
                    IsEnabledCheckoutButton = true;
                }
                IsValidAmount = true;
                OnPropertyChanged(); 
            } 
        }

        private int _discount;
        public int Discount 
        {
            get => _discount; 
            set
            { 
                _discount = value;
                if (IsValidAmount == true && IsValidGiftAmount == true)
                {
                    IsEnabledCheckoutButton = true;
                }
                IsValidDiscount = true;
                OnPropertyChanged(); 
            } 
        }

        private int _giftAmount;
        public int GiftAmount 
        { 
            get => _giftAmount; 
            set 
            { 
                _giftAmount = value;
                if (IsValidDiscount == true && IsValidAmount == true)
                {
                    IsEnabledCheckoutButton = true;
                }
                IsValidGiftAmount = true;
                OnPropertyChanged(); 
            } 
        }

        private string _summaryPrice;
        public string SummaryPrice { get => _summaryPrice; set { _summaryPrice = value; OnPropertyChanged(); } }
        #endregion

        #region command
        public ICommand DisableAmount { get; set; }
        public ICommand DisableDiscount { get; set; }
        public ICommand DisableGiftAmount { get; set; }
        #endregion
        public DetailInList()
        {
            DisableAmount = new RelayCommand<dynamic>((param) => { return true; }, (param) => {
                IsValidAmount = false;
                IsEnabledCheckoutButton = false;
            });
            DisableDiscount = new RelayCommand<dynamic>((param) => { return true; }, (param) => {
                IsValidDiscount = false;
                IsEnabledCheckoutButton = false;
            });
            DisableGiftAmount = new RelayCommand<dynamic>((param) => { return true; }, (param) => {
                IsValidGiftAmount = false;
                IsEnabledCheckoutButton = false;
            });
        }
    }
    public class IsPercentRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                if (((string)value).Length > 0)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch((string)value, "^[0-9]+$"))
                    {
                        if (((string)value).Length > 3 || (((string)value).Length == 3 && (string)value != "100"))
                        {
                            return new ValidationResult(false, "Vui lòng nhập số từ 0 đến 100");
                        }
                        else
                        {
                            return ValidationResult.ValidResult;
                        }
                    }
                    else
                    {
                        return new ValidationResult(false, "Vui lòng nhập trường này chỉ bao gồm kí tự số");
                    }
                }
                else
                {
                    return new ValidationResult(false, "Vui lòng không bỏ trống trường này");
                }
            }
            catch (Exception e)
            {
                return new ValidationResult(false, e.Message);
            }

        }
    }
    public class IsNotNullRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                if (value != null)
                {
                    return ValidationResult.ValidResult;
                }
                else
                {
                    return new ValidationResult(false, "Vui lòng không bỏ trống trường này");
                }
            }
            catch (Exception e)
            {
                return new ValidationResult(false, e.Message);
            }

        }
    }
}

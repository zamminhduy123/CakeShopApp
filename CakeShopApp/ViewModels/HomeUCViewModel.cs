using CakeShopApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CakeShopApp.Models;
using System.Windows.Input;

namespace CakeShopApp.ViewModels
{
    class HomeUCViewModel : BaseViewModel
    {
        #region variables

        #endregion

        #region properties

        private AsyncObservableCollection<dynamic> _products;
        public AsyncObservableCollection<dynamic> Products { get => _products; set { _products = value; OnPropertyChanged(); } }

        private dynamic _selectedProduct;
        public dynamic SelectedProduct
        {
            get => _selectedProduct; set
            {
                _selectedProduct = value;
                if (SelectedProduct != null)
                {
                    InvoiceDetails.Add(new DetailInList
                    {
                        ProductId = SelectedProduct.Id,
                        ProductName = SelectedProduct.Name,
                        ProductThumbnail = SelectedProduct.Thumbnail,
                        ProductPrice = SelectedProduct.Price,
                        Amount = 1,
                        Discount = 0,
                        GiftAmount = 0,
                        SummaryPrice = SelectedProduct.Price,
                    });
                    Products.Remove(SelectedProduct);
                    SelectedProduct = null;
                }
                OnPropertyChanged();
            }
        }

        private AsyncObservableCollection<DetailInList> _invoiceDetails;
        public AsyncObservableCollection<DetailInList> InvoiceDetails { get => _invoiceDetails; set { _invoiceDetails = value; OnPropertyChanged(); } }

        #endregion

        #region commands
        public ICommand DeleteDetailInListCommand { get; set; }
        #endregion

        public HomeUCViewModel()
        {
            // khởi tạo dữ liệu
            Products = new AsyncObservableCollection<dynamic>();
            foreach (var product in DataProvider.Ins.DB.Products)
            {
                Products.Add(new
                {
                    Id = product.Id,
                    Name = product.Name,
                    Thumbnail = (product.Photos.Count == 0) ? null : product.Photos.ToList()[0].ImageBytes,
                    Price = product.SellPrice.ToString(),
                });
            }
            InvoiceDetails = new AsyncObservableCollection<DetailInList>();

            // Commands
            DeleteDetailInListCommand = new RelayCommand<DetailInList>((param) => { return true; }, (param) => {
                int count = InvoiceDetails.Count;
                Product product = DataProvider.Ins.DB.Products.Find(param.ProductId);
                int index = DataProvider.Ins.DB.Products.ToList().IndexOf(product);
                Products.Insert(index, new
                {
                    Id = param.ProductId,
                    Name = param.ProductName,
                    Thumbnail = param.ProductThumbnail,
                    Price = param.ProductPrice,
                });
                InvoiceDetails.Remove(param);
            });
        }

    }
    // tạo class hỗ trợ trình bày dữ liệu lên view
    public class DetailInList
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public byte[] ProductThumbnail { get; set; }
        public string ProductPrice { get; set; }
        public int Amount { get; set; }
        public int Discount { get; set; }
        public int GiftAmount { get; set; }
        public string SummaryPrice { get; set; }
    }
}

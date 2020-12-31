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
        private List<string> _listSort = new List<string>() { 
            "Bán chạy",
            "Hàng mới",
            "A-Z",
            "Z-A",
            "Giá tăng dần",
            "Giá giảm dần",
        };
        #endregion

        #region properties
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
                    LoadProducts();
                }
                SelectedProduct = null;
                OnPropertyChanged();
            }
        }

        private AsyncObservableCollection<string> _sorts;
        public AsyncObservableCollection<string> Sorts { get => _sorts; set { _sorts = value; OnPropertyChanged(); } }

        private string _selectedSort;
        public string SelectedSort
        {
            get => _selectedSort; set
            {
                _selectedSort = value;
                if (SelectedCategory != null && SelectedSort != null)
                {
                    LoadProducts();
                }
                SelectedProduct = null;
                OnPropertyChanged();
            }
        }

        // make invoice

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
            InvoiceDetails = new AsyncObservableCollection<DetailInList>();

            Categories = new AsyncObservableCollection<dynamic>();
            Categories.Add(new { 
                Id = 0,
                Name = "Tất cả",
            });
            foreach (var category in DataProvider.Ins.DB.Categories)
            {
                Categories.Add(new
                {
                    Id = category.Id,
                    Name = category.Name,
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
            DeleteDetailInListCommand = new RelayCommand<DetailInList>((param) => { return true; }, (param) => {
                InvoiceDetails.Remove(param);
                LoadProducts();
            });
        }
        void LoadProducts()
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

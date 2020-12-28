using CakeShopApp.Model;
using CakeShopApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CakeShopApp.ViewModels
{
    class CakesUCViewModel : BaseViewModel
    {
        #region variables

        #endregion

        #region properties

        private AsyncObservableCollection<Root> _categories;
        public AsyncObservableCollection<Root> Categories { get => _categories; set { _categories = value; OnPropertyChanged(); } }

        private Root _selectedCategory;
        public Root SelectedCategory
        {
            get => _selectedCategory; set
            {
                _selectedCategory = value;
                Products = new AsyncObservableCollection<dynamic>();
                if (SelectedCategory.Id == 0)
                {
                    foreach (var product in DataProvider.Ins.DB.Products)
                    {
                        Products.Add(new {
                            Id = product.Id,
                            Name = product.Name,
                            Thumbnail = (product.Photos.Count == 0) ? null: product.Photos.ToList()[0].ImageBytes,
                            Price = product.SellPrice.ToString(),
                            ImportPrice = product.ImportPrice.ToString(),
                            Description = product.Description,
                            InStock = product.InStockAmount,
                            CategoryId = product.CategoryId,
                        });
                    }
                }
                else
                {
                    int id = SelectedCategory.Id;
                    foreach (var product in DataProvider.Ins.DB.Products.Where(x => x.CategoryId == id))
                    {
                        Products.Add(new
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Thumbnail = (product.Photos.Count == 0) ? null : product.Photos.ToList()[0].ImageBytes,
                            Price = product.SellPrice.ToString(),
                            ImportPrice = product.ImportPrice.ToString(),
                            Description = product.Description,
                            InStock = product.InStockAmount,
                            CategoryId = product.CategoryId,
                        });
                    }
                }
                OnPropertyChanged();
            }
        }

        private dynamic _selectedProduct;
        public dynamic SelectedProduct
        {
            get => _selectedProduct; set
            {
                _selectedProduct = value;
                
                OnPropertyChanged();
            }
        }

        private AsyncObservableCollection<dynamic> _products;
        public AsyncObservableCollection<dynamic> Products { get => _products; set { _products = value; OnPropertyChanged(); } }

        #endregion

        #region commands
        public ICommand ChangeCategoryCommand { get; set; }
        public ICommand AddProductCommand { get; set; }
        public ICommand EditProductCommand { get; set; }
        public ICommand DeleteProductCommand { get; set; }
        #endregion

        public CakesUCViewModel()
        {
            // khởi tạo dữ liệu
            Categories = new AsyncObservableCollection<Root>();
            Categories.Add(new Root { Id = 0, Name = "Tất cả", Count = DataProvider.Ins.DB.Products.Count()});
            foreach (var category in DataProvider.Ins.DB.Categories)
            {
                Categories.Add(new Root { 
                    Id = category.Id,
                    Name = category.Name,
                    Count = DataProvider.Ins.DB.Products.Where(x => x.CategoryId == category.Id).Count()
                });
            }
            foreach (var category in Categories)
            {
                category.LoadChild();
            }

            SelectedCategory = Categories.First(x => x.Id == 0);

            // commands
            ChangeCategoryCommand = new RelayCommand<object>((param) => { return true; }, (param) => {
                Products = new AsyncObservableCollection<dynamic>();
                if (param is Root)
                {
                    var tmp = param as Root;
                    if (tmp.Id == 0)
                    {
                        foreach (var product in DataProvider.Ins.DB.Products)
                        {
                            Products.Add(new
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Thumbnail = (product.Photos.Count == 0) ? null : product.Photos.ToList()[0].ImageBytes,
                                Price = product.SellPrice.ToString(),
                                ImportPrice = product.ImportPrice.ToString(),
                                Description = product.Description,
                                InStock = product.InStockAmount,
                                CategoryId = product.CategoryId,
                            });
                        }
                    }
                    else
                    {
                        foreach (var product in DataProvider.Ins.DB.Products.Where(x => x.CategoryId == tmp.Id))
                        {
                            Products.Add(new
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Thumbnail = (product.Photos.Count == 0) ? null : product.Photos.ToList()[0].ImageBytes,
                                Price = product.SellPrice.ToString(),
                                ImportPrice = product.ImportPrice.ToString(),
                                Description = product.Description,
                                InStock = product.InStockAmount,
                                CategoryId = product.CategoryId,
                            });
                        }
                    }
                }
                else if (param is RootChild)
                {
                    var tmp = param as RootChild;
                    Product product = DataProvider.Ins.DB.Products.Find(tmp.Id);
                    SelectedProduct = new {
                        Id = product.Id,
                        Name = product.Name,
                        Thumbnail = (product.Photos.Count == 0) ? null : product.Photos.ToList()[0].ImageBytes,
                        Price = product.SellPrice.ToString(),
                    };
                }
            });
            AddProductCommand = new RelayCommand<object>((param) => { return true; }, (param) => { });
            EditProductCommand = new RelayCommand<dynamic>((param) => { return true; }, (param) => { });
            DeleteProductCommand = new RelayCommand<dynamic>((param) => { return true; }, (param) => {
                if (Global.GetInstance().ConfirmMessageDelete() == true)
                {
                    // Xóa khỏi Phân loại tất cả
                    Root all = Categories.First(x => x.Id == 0);
                    all.Count--;
                    all.Child.Remove(all.Child.First(x => x.Id == param.Id));

                    // Xóa khỏi Phân loại bánh chính
                    Root category = Categories.First(x => x.Id == param.CategoryId);
                    category.Count--;
                    category.Child.Remove(category.Child.First(x => x.Id == param.Id));

                    // Xóa khỏi Database
                    DataProvider.Ins.DB.Products.Remove(DataProvider.Ins.DB.Products.Find(param.Id));
                    DataProvider.Ins.DB.SaveChanges();

                    // Xóa khỏi danh sách hiển thị
                    Products.Remove(param);
                }
            });
        }
    }

    //tạo class hỗ trợ
    public class Root : BaseViewModel
    {

        private int _id;
        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }

        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

        private int _count;
        public int Count { get => _count; set { _count = value; OnPropertyChanged(); } }

        public AsyncObservableCollection<RootChild> Child { get; set; }
        public void LoadChild()
        {
            Child = new AsyncObservableCollection<RootChild>();
            if (Id == 0)
            {
                foreach (var product in DataProvider.Ins.DB.Products)
                {
                    Child.Add(new RootChild
                    {
                        Id = product.Id,
                        Name = product.Name,
                    });
                }
            }
            else
            {
                foreach (var product in DataProvider.Ins.DB.Products.Where(x => x.CategoryId == Id))
                {
                    Child.Add(new RootChild
                    {
                        Id = product.Id,
                        Name = product.Name,
                    });
                }
            }
        }
    }
    public class RootChild : BaseViewModel
    {

        private int _id;
        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }

        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

    }
}

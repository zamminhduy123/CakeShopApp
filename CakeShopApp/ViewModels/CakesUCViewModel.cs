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
        }
    }

    //tạo class hỗ trợ
    public class Root
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
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
    public class RootChild
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

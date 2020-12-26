using CakeShopApp.Model;
using CakeShopApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeShopApp.ViewModels
{
    class CakesUCViewModel : BaseViewModel
    {
        #region variables

        #endregion

        #region properties

        private AsyncObservableCollection<dynamic> _categories;
        public AsyncObservableCollection<dynamic> Categories { get => _categories; set { _categories = value; OnPropertyChanged(); } }

        private dynamic _selectedCategory;
        public dynamic SelectedCategory
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

        #endregion

        public CakesUCViewModel()
        {
            // khởi tạo dữ liệu
            Categories = new AsyncObservableCollection<dynamic>();
            Categories.Add(new { Id = 0, Name = "Tất cả", Count = DataProvider.Ins.DB.Products.Count(), });
            foreach (var category in DataProvider.Ins.DB.Categories)
            {
                Categories.Add(new { 
                Id = category.Id,
                Name = category.Name,
                Count = category.Products.Count(),
                });
            }
            SelectedCategory = Categories.First(x => x.Id == 0);
        }
    }
}

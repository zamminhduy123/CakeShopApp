using CakeShopApp.Model;
using CakeShopApp.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CakeShopApp.ViewModels
{
    class CakesUCViewModel : BaseViewModel
    {
        #region variables
        Product _editProduct;
        #endregion

        #region properties

        // SHOW LIST AND SELECTED ITEM

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

        // ADD NEW PRODUCT / UPDATE PRODUCT

        private dynamic _selectedNewProductCategory;
        public dynamic SelectedNewProductCategory { get => _selectedNewProductCategory; set { _selectedNewProductCategory = value; OnPropertyChanged(); } }

        private AsyncObservableCollection<dynamic> _newProductCategoriesList;
        public AsyncObservableCollection<dynamic> NewProductCategoriesList { get => _newProductCategoriesList; set { _newProductCategoriesList = value; OnPropertyChanged(); } }

        private string _newProductName;
        public string NewProductName { get => _newProductName; set { _newProductName = value; OnPropertyChanged(); } }

        private string _newProductDescription;
        public string NewProductDescription { get => _newProductDescription; set { _newProductDescription = value; OnPropertyChanged(); } }

        private int _newProductImportAmount;
        public int NewProductImportAmount { get => _newProductImportAmount; set { _newProductImportAmount = value; OnPropertyChanged(); } }

        private int _newProductImportPrice;
        public int NewProductImportPrice { get => _newProductImportPrice; set { _newProductImportPrice = value; OnPropertyChanged(); } }

        private int _newProductSellPrice;
        public int NewProductSellPrice { get => _newProductSellPrice; set { _newProductSellPrice = value; OnPropertyChanged(); } }

        private bool _isOpenAddProductDialog;
        public bool IsOpenAddProductDialog
        {
            get => _isOpenAddProductDialog;
            set
            {
                _isOpenAddProductDialog = value;
                NewProductName = null;
                SelectedNewProductCategory = null;
                NewProductDescription = null;
                NewProductImportAmount = 0;
                NewProductImportPrice = 0;
                NewProductSellPrice = 0;
                NewProductImages = new AsyncObservableCollection<dynamic>();

                NewProductCategoriesList = new AsyncObservableCollection<dynamic>();
                foreach (var category in DataProvider.Ins.DB.Categories)
                {
                    NewProductCategoriesList.Add(new Root
                    {
                        Id = category.Id,
                        Name = category.Name,
                    });
                }
                OnPropertyChanged();
            }
        }

        private bool _isOpenUpdateProductDialog;
        public bool IsOpenUpdateProductDialog
        {
            get => _isOpenUpdateProductDialog;
            set
            {
                _isOpenUpdateProductDialog = value;
                if (value == true)
                {
                    NewProductCategoriesList = new AsyncObservableCollection<dynamic>();
                    foreach (var category in DataProvider.Ins.DB.Categories)
                    {
                        NewProductCategoriesList.Add(new Root
                        {
                            Id = category.Id,
                            Name = category.Name,
                        });
                    }
                }
                else
                {
                    _editProduct = null;
                }
                OnPropertyChanged();
            }
        }

        private AsyncObservableCollection<dynamic> _newProductImages;
        public AsyncObservableCollection<dynamic> NewProductImages { get => _newProductImages; set { _newProductImages = value; OnPropertyChanged(); } }

        private dynamic _selectedNewProductImage;
        public dynamic SelectedNewProductImage { get => _selectedNewProductImage; set { _selectedNewProductImage = value; OnPropertyChanged(); } }

        #endregion

        #region commands
        public ICommand ChangeCategoryCommand { get; set; }
        public ICommand AddProductCommand { get; set; }
        public ICommand ClickAddProductButtonCommand { get; set; }
        public ICommand EditProductCommand { get; set; }
        public ICommand ClickEditProductButtonCommand { get; set; }
        public ICommand DeleteProductCommand { get; set; }
        public ICommand AddImageCommand { get; set; }
        public ICommand DeleteImageCommand { get; set; }
        public ICommand SetThumbnailCommand { get; set; }
        
        #endregion

        public CakesUCViewModel()
        {
            // khởi tạo dữ liệu
            Categories = new AsyncObservableCollection<Root>();
            Categories.Add(new Root { Id = 0, Name = "Tất cả", Count = DataProvider.Ins.DB.Products.Where(x => x.IsHidden == 0).Count()});
            foreach (var category in DataProvider.Ins.DB.Categories)
            {
                Categories.Add(new Root { 
                    Id = category.Id,
                    Name = category.Name,
                    Count = DataProvider.Ins.DB.Products.Where(x => x.IsHidden == 0 && x.CategoryId == category.Id).Count()
                });
            }
            foreach (var category in Categories)
            {
                category.LoadChild();
            }

            SelectedCategory = Categories.First(x => x.Id == 0);

            NewProductImages = new AsyncObservableCollection<dynamic>();
            // commands
            ChangeCategoryCommand = new RelayCommand<object>((param) => { return true; }, (param) => {
                Products = new AsyncObservableCollection<dynamic>();
                if (param is Root)
                {
                    var tmp = param as Root;
                    if (tmp.Id == 0)
                    {
                        foreach (var product in DataProvider.Ins.DB.Products.Where(x => x.IsHidden == 0))
                        {
                            Products.Add(new
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Thumbnail = (product.Photos.Count == 0) ? null : product.Photos.ToList()[0].ImageBytes,
                                Price = product.SellPrice.ToString(),
                                ImportPrice = product.ImportPrice.ToString(),
                                InStock = product.InStockAmount,
                                CategoryId = product.CategoryId,
                            });
                        }
                    }
                    else
                    {
                        foreach (var product in DataProvider.Ins.DB.Products.Where(x => x.IsHidden == 0 && x.CategoryId == tmp.Id))
                        {
                            Products.Add(new
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Thumbnail = (product.Photos.Count == 0) ? null : product.Photos.ToList()[0].ImageBytes,
                                Price = product.SellPrice.ToString(),
                                ImportPrice = product.ImportPrice.ToString(),
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
            ClickAddProductButtonCommand = new RelayCommand<dynamic>((param) => { return true; }, (param) => {
                IsOpenAddProductDialog = true;
            });
            AddProductCommand = new RelayCommand<string>((param) => { return true; }, (param) => {
                if (bool.Parse(param) == true)
                {
                    Product newproduct = new Product();
                    newproduct.CategoryId = SelectedNewProductCategory.Id;
                    newproduct.Name = NewProductName;
                    newproduct.Description = (NewProductDescription == null) ? "" : NewProductDescription;
                    newproduct.ImportDate = DateTime.Now;
                    newproduct.ImportAmount = NewProductImportAmount;
                    newproduct.ImportPrice = NewProductImportPrice;
                    newproduct.InStockAmount = NewProductImportAmount;
                    newproduct.SellPrice = NewProductSellPrice;
                    newproduct.IsHidden = 0;
                    DataProvider.Ins.DB.Products.Add(newproduct);
                    DataProvider.Ins.DB.SaveChanges();
                    if (NewProductImages.Count != 0)
                    {
                        DataProvider.Ins.DB.Photos.Add(new Photo
                        {
                            ProductId = newproduct.Id,
                            OrderNumber = 0,
                            ImageBytes = NewProductImages.First(x => x.IsThumbnail == true).ByteImage,
                        });
                        foreach (var image in NewProductImages.Where(x => x.IsThumbnail == false))
                        {
                            var maxordernum = 1;
                            DataProvider.Ins.DB.Photos.Add(new Photo
                            {
                                ProductId = newproduct.Id,
                                OrderNumber = maxordernum++,
                                ImageBytes = image.ByteImage,
                            });
                        }
                    }
                    DataProvider.Ins.DB.SaveChanges();

                    // Thêm vào Phân loại tất cả
                    Root all = Categories.First(x => x.Id == 0);
                    all.Count++;
                    all.Child.Add(new RootChild { Id = newproduct.Id, Name = newproduct.Name});

                    // Thêm vào Phân loại bánh chính
                    Root category = Categories.First(x => x.Id == newproduct.CategoryId);
                    category.Count++;
                    category.Child.Add(new RootChild { Id = newproduct.Id, Name = newproduct.Name });

                    //Thêm vào danh sách nếu đang hiển thị phân loại đó
                    if (SelectedCategory.Id == 0 || SelectedCategory.Id == newproduct.CategoryId)
                    {
                        Products.Add(new
                        {
                            Id = newproduct.Id,
                            Name = newproduct.Name,
                            Thumbnail = (newproduct.Photos.Count == 0) ? null : newproduct.Photos.ToList()[0].ImageBytes,
                            Price = newproduct.SellPrice.ToString(),
                            ImportPrice = newproduct.ImportPrice.ToString(),
                            InStock = newproduct.InStockAmount,
                            CategoryId = newproduct.CategoryId,
                        });
                    }
                }
                IsOpenAddProductDialog = false;
            });
            ClickEditProductButtonCommand = new RelayCommand<dynamic>((param) => { return true; }, (param) => {
                IsOpenUpdateProductDialog = true;
                _editProduct = DataProvider.Ins.DB.Products.Find(param);
                NewProductName = _editProduct.Name;
                SelectedNewProductCategory = NewProductCategoriesList.First(x => x.Id == _editProduct.CategoryId);
                NewProductDescription = _editProduct.Description;
                NewProductImportAmount = 0;
                NewProductImportPrice = _editProduct.ImportPrice;
                NewProductSellPrice = _editProduct.SellPrice;
                NewProductImages = new AsyncObservableCollection<dynamic>();
                foreach (var image in _editProduct.Photos)
                {
                    NewProductImages.Add(new { 
                        IsThumbnail = (image.OrderNumber == 0) ? true : false, 
                        ByteImage = image.ImageBytes, });
                }
            }); 
            EditProductCommand = new RelayCommand<dynamic>((param) => { return true; }, (param) => {
                if (bool.Parse(param) == true)
                {
                    // xóa dữ liệu cũ
                    _editProduct.IsHidden = 1;
                    DataProvider.Ins.DB.SaveChanges();

                    // tạo dữ liệu mới
                    Product newproduct = new Product();
                    newproduct.CategoryId = SelectedNewProductCategory.Id;
                    newproduct.Name = NewProductName;
                    newproduct.Description = (NewProductDescription == null) ? "" : NewProductDescription;
                    newproduct.ImportDate = DateTime.Now;
                    newproduct.ImportAmount = NewProductImportAmount + _editProduct.ImportAmount;
                    newproduct.ImportPrice = NewProductImportPrice;
                    newproduct.InStockAmount = NewProductImportAmount + _editProduct.InStockAmount;
                    newproduct.SellPrice = NewProductSellPrice;
                    newproduct.IsHidden = 0;
                    DataProvider.Ins.DB.Products.Add(newproduct);
                    DataProvider.Ins.DB.SaveChanges();
                    if (NewProductImages.Count != 0)
                    {
                        DataProvider.Ins.DB.Photos.Add(new Photo
                        {
                            ProductId = newproduct.Id,
                            OrderNumber = 0,
                            ImageBytes = NewProductImages.First(x => x.IsThumbnail == true).ByteImage,
                        });
                        foreach (var image in NewProductImages.Where(x => x.IsThumbnail == false))
                        {
                            var maxordernum = 1;
                            DataProvider.Ins.DB.Photos.Add(new Photo
                            {
                                ProductId = newproduct.Id,
                                OrderNumber = maxordernum++,
                                ImageBytes = image.ByteImage,
                            });
                        }
                    }
                    DataProvider.Ins.DB.SaveChanges();

                    // Xóa khỏi Phân loại tất cả
                    Root all = Categories.First(x => x.Id == 0);
                    all.Count--;
                    all.Child.Remove(all.Child.First(x => x.Id == _editProduct.Id));

                    // Xóa khỏi Phân loại bánh chính
                    Root category = Categories.First(x => x.Id == _editProduct.CategoryId);
                    category.Count--;
                    category.Child.Remove(category.Child.First(x => x.Id == _editProduct.Id));

                    // Xóa khỏi danh sách hiển thị
                    Products.Remove(Products.First(x => x.Id == _editProduct.Id));

                    // Thêm vào Phân loại tất cả
                    Root all2 = Categories.First(x => x.Id == 0);
                    all2.Count++;
                    all2.Child.Add(new RootChild { Id = newproduct.Id, Name = newproduct.Name });

                    // Thêm vào Phân loại bánh chính
                    Root category2 = Categories.First(x => x.Id == newproduct.CategoryId);
                    category2.Count++;
                    category2.Child.Add(new RootChild { Id = newproduct.Id, Name = newproduct.Name });

                    //Thêm vào danh sách nếu đang hiển thị phân loại đó
                    if (SelectedCategory.Id == 0 || SelectedCategory.Id == newproduct.CategoryId)
                    {
                        Products.Add(new
                        {
                            Id = newproduct.Id,
                            Name = newproduct.Name,
                            Thumbnail = (newproduct.Photos.Count == 0) ? null : newproduct.Photos.ToList()[0].ImageBytes,
                            Price = newproduct.SellPrice.ToString(),
                            ImportPrice = newproduct.ImportPrice.ToString(),
                            InStock = newproduct.InStockAmount,
                            CategoryId = newproduct.CategoryId,
                        });
                    }
                }
                IsOpenUpdateProductDialog = false;
            });
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
                    DataProvider.Ins.DB.Products.Find(param.Id).IsHidden = 1;
                    DataProvider.Ins.DB.SaveChanges();

                    // Xóa khỏi danh sách hiển thị
                    Products.Remove(param);
                }
            });
            AddImageCommand = new RelayCommand<dynamic>((param) => { return true; }, (param) =>
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = true;
                dialog.Filter = "JPG files (*.jpg)|*.jpg| PNG files (*.png)|*.png| All files (*.*)|*.*";
                if (dialog.ShowDialog() == true)
                {
                    bool hasthumbnail = (NewProductImages.Where(x => x.IsThumbnail == true).Count() == 1) ? true : false;
                    foreach (var absoluteLink in dialog.FileNames)
                    {
                        byte[] newBytesImage = BitMapImageTOBytes(new BitmapImage(
                                                    new Uri(absoluteLink,
                                                    UriKind.Absolute)
                                                    ));
                        NewProductImages.Add(new
                        {
                            IsThumbnail = !hasthumbnail,
                            ByteImage = newBytesImage,
                        });
                        hasthumbnail = true;
                    }
                }
            });
            DeleteImageCommand = new RelayCommand<dynamic>((param) => { return true; }, (param) =>
            {
                if (SelectedNewProductImage.IsThumbnail == true && NewProductImages.Count != 0)
                {
                    int setthumbnailindex = 0;
                    if (NewProductImages.IndexOf(SelectedNewProductImage) == 0)
                    {
                        setthumbnailindex = 1;
                    }
                    dynamic tmp = new { IsThumbnail = true, ByteImage = NewProductImages.First().ByteImage };
                    NewProductImages.RemoveAt(setthumbnailindex);
                    NewProductImages.Insert(setthumbnailindex, tmp);
                }
                NewProductImages.Remove(SelectedNewProductImage);
            });
            SetThumbnailCommand = new RelayCommand<dynamic>((param) => { return true; }, (param) =>
            {
                int index;
                int hasThumbnail = NewProductImages.Where(x => x.IsThumbnail == true).Count();
                dynamic tmp;
                if (hasThumbnail != 0)
                {
                    tmp = NewProductImages.First(x => x.IsThumbnail == true);
                    index = NewProductImages.IndexOf(tmp);
                    dynamic changedtmp = new { IsThumbnail = false, ByteImage = tmp.ByteImage };
                    NewProductImages.Remove(tmp);
                    NewProductImages.Insert(index, changedtmp);
                }
                tmp = new { IsThumbnail = true, ByteImage = SelectedNewProductImage.ByteImage };
                index = NewProductImages.IndexOf(SelectedNewProductImage);
                NewProductImages.Remove(SelectedNewProductImage);
                NewProductImages.Insert(index, tmp);
            });
        }

        public byte[] BitMapImageTOBytes(BitmapImage imageC)
        {
            if (imageC == null) return null;
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.ToArray();
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
                foreach (var product in DataProvider.Ins.DB.Products.Where(x => x.IsHidden == 0))
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
                foreach (var product in DataProvider.Ins.DB.Products.Where(x => x.IsHidden == 0 && x.CategoryId == Id))
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

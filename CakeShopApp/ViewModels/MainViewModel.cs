using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Configuration;

namespace CakeShopApp.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        #region private variables
        //private BaseViewModel _currentPageViewModel = null;
        //private Visibility _leftPanelVisibility = Visibility.Visible;
        private String _addPlaceColor = Brushes.White.ToString();
        private String _addMemberColor = Brushes.White.ToString();
        private String _settingColor = Brushes.White.ToString();
        private String _aboutColor = Brushes.White.ToString();
        private String _versionTextBlock = Brushes.White.ToString();
        #endregion

        #region Commands

        public ICommand HomeCommand { get; set; }
        public ICommand CheckOutCommand { get; set; }
        public ICommand StatisticCommand { get; set; }
        public ICommand CakesCommand { get; set; }
        public ICommand SettingCommand { get; set; }
        public ICommand AboutCommand { get; set; }
        public ICommand OpenPanelCommand { get; set; }
        public ICommand InvoiceListCommand { get; set; }
        #endregion

        #region Panel

        public String VersionTextBlock { get => _versionTextBlock; set { _versionTextBlock = value; OnPropertyChanged(); } }

        public Global global =  Global.GetInstance();

        #endregion

        private string GetPublishedVersion()
        {
            var version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            string appVersion = $"{version.Major}.{version.Minor}";
            return appVersion;
        }


        #region static variable

        public static bool IsShowed = false;

        #endregion

        public MainViewModel()
        {
            ResetPanelColor();
            global.HomeColor = Brushes.SaddleBrown.ToString();
            global.HomeTextColor = Brushes.White.ToString();


            VersionTextBlock = GetPublishedVersion();
            if (VersionTextBlock == null || VersionTextBlock == "")
                VersionTextBlock = "not installed";

            HomeCommand = new RelayCommand<object>((param) => { return true; }, (param) =>
            {
                ResetPanelColor();
                global.HomeColor = Brushes.SaddleBrown.ToString();
                global.HomeTextColor = Brushes.White.ToString();
                global.CurrentPageViewModel = new HomeUCViewModel();
                
            });

            InvoiceListCommand = new RelayCommand<object>((param) => { return true; }, (param) =>
            {
                ResetPanelColor();
                global.InvoiceListColor = Brushes.SaddleBrown.ToString();
                global.InvoiceListTextColor = Brushes.White.ToString();
                global.CurrentPageViewModel = new InvoiceListUCViewModel();
            });

            CakesCommand = new RelayCommand<object>((param) => { return true; }, (param) =>
            {
                ResetPanelColor();
                global.CakesColor = Brushes.SaddleBrown.ToString();
                global.CakesTextColor = Brushes.White.ToString();
                global.CurrentPageViewModel = new CakesUCViewModel();
            });

            StatisticCommand = new RelayCommand<object>((param) => { return true; }, (param) =>
            {
                ResetPanelColor();
                global.StatisticColor = Brushes.SaddleBrown.ToString();
                global.StatisticTextColor = Brushes.White.ToString();
                global.CurrentPageViewModel = new StatisticsUCViewModel();
            });

            SettingCommand = new RelayCommand<object>((param) => { return true; }, (param) =>
            {
                ResetPanelColor();
                global.SettingColor = Brushes.SaddleBrown.ToString();
                global.SettingTextColor = Brushes.White.ToString();
                global.CurrentPageViewModel = new SettingUCViewModel();
            });
        }

        void ResetPanelColor()
        {
            global.HomeColor = Brushes.White.ToString();
            global.HomeTextColor = Brushes.Gray.ToString();

            global.CheckOutColor = Brushes.White.ToString();
            global.CheckOutTextColor = Brushes.Gray.ToString();

            global.StatisticColor = Brushes.White.ToString();
            global.StatisticTextColor = Brushes.Gray.ToString();

            global.SettingColor = Brushes.White.ToString();
            global.SettingTextColor = Brushes.Gray.ToString();

            global.CakesColor = Brushes.White.ToString();
            global.CakesTextColor = Brushes.Gray.ToString();

            global.InvoiceListColor = Brushes.White.ToString();
            global.InvoiceListTextColor = Brushes.Gray.ToString();
        }
    }
}

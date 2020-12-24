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
        private Visibility _leftPanelVisibility = Visibility.Visible;
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
        public ICommand AddMemberCommand { get; set; }
        public ICommand SettingCommand { get; set; }
        public ICommand AboutCommand { get; set; }
        public ICommand OpenPanelCommand { get; set; }

        #endregion

        #region PanelColor

        public String PlacesColor { get => _addPlaceColor; set { _addPlaceColor = value; OnPropertyChanged(); } }
        public String AddMemberColor { get => _addMemberColor; set { _addMemberColor = value; OnPropertyChanged(); } }
        public String SettingColor { get => _settingColor; set { _settingColor = value; OnPropertyChanged(); } }
        public String AboutColor { get => _aboutColor; set { _aboutColor = value; OnPropertyChanged(); } }

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
            //global.CurrentPageViewModel = new HomeUCViewModel();

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

            CheckOutCommand = new RelayCommand<object>((param) => { return true; }, (param) =>
            {
                ResetPanelColor();
                global.CheckOutColor = Brushes.SaddleBrown.ToString();
                global.CheckOutTextColor = Brushes.White.ToString();
            });

            StatisticCommand = new RelayCommand<object>((param) => { return true; }, (param) =>
            {
                ResetPanelColor();
                global.StatisticColor = Brushes.SaddleBrown.ToString();
                global.StatisticTextColor = Brushes.White.ToString();
            });

            SettingCommand = new RelayCommand<object>((param) => { return true; }, (param) =>
            {
                ResetPanelColor();
                global.SettingColor = Brushes.SaddleBrown.ToString();
                global.SettingTextColor = Brushes.White.ToString();
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
        }
    }
}

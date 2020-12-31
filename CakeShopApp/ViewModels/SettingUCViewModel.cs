using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeShopApp.ViewModels
{
    class SettingUCViewModel : BaseViewModel
    {
        private bool _isShowSplash;
        public bool IsShowSplash
        {
            get => _isShowSplash; set
            {
                _isShowSplash = value;
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["ShowSplashScreen"].Value = IsShowSplash.ToString();
                config.Save(ConfigurationSaveMode.Minimal);

                ConfigurationManager.RefreshSection("appSettings");
                OnPropertyChanged();
            }
        }

        public SettingUCViewModel()
        {
            var value = ConfigurationManager.AppSettings["ShowSplashScreen"];
            IsShowSplash = bool.Parse(value);
        }

    }
}

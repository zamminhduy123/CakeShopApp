using System;
using System.Timers;
using System.Windows;
using CakeShopApp;
using CakeShopApp.ViewModels;

namespace CakeShopApp
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        private const int Interval = 4000;
        private readonly Timer dT = new Timer(Interval);
        public SplashScreen()
        {
            InitializeComponent();
            this.DataContext = new SplashScreenViewModel();
            dT.Elapsed += dt_Tick;
            dT.Start();
        }
        void dt_Tick(object sender, EventArgs e)
        {
            dT.Dispose();
            Dispatcher.Invoke(() =>
            {
                if (MainViewModel.IsShowed == false)
                {
                    MainWindow mW = new MainWindow();
                    mW.Show();
                    MainViewModel.IsShowed = true;
                }
                this.Close();
            });


        }
    }
}
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace prototipoGPS
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage()); // Reemplaza 'MainPage' por el nombre de tu página principal  
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

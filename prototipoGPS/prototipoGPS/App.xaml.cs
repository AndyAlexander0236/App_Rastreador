using prototipoGPS.Modelos;
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

            // Cargar las preferencias de la configuración al iniciar la app
            Configuracion.CargarDesdePreferencias();

            // Cambiar la IP globalmente
            Configuracion.Ip = "192.168.1.49"; 


            MainPage = new NavigationPage(new MainPage()); 
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

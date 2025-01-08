using prototipoGPS.Modelos;
using System;
using Xamarin.Essentials;
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
            //Configuracion.Ip = "192.168.104.147";



            MainPage = new NavigationPage(new MainPage()); 
        }

        protected override void OnStart()
        {
            // Verificar si el usuario ya inició sesión
            bool isLoggedIn = Preferences.Get("IsLoggedIn", false);

            if (isLoggedIn)
            {
                // Redirigir a la pantalla de entrada
                MainPage = new NavigationPage(new entrada());
            }
            else
            {
                // Mostrar la pantalla principal
                MainPage = new NavigationPage(new MainPage());
            }
        }


        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

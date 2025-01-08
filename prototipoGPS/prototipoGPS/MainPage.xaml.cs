using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace prototipoGPS
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        // Evento para el botón "Continuar con Trace"  
        private async void OnContinueClicked(object sender, EventArgs e)
        {
            bool isLoggedIn = Preferences.Get("IsLoggedIn", false);

            if (isLoggedIn)
            {
                // Redirigir a la pantalla de entrada si está logueado
                await Navigation.PushAsync(new entrada());
            }
            else
            {
                // Redirigir al inicio de sesión si no está logueado
                await Navigation.PushAsync(new login());
            }
        }

     
        // Evento para el botón "Registrarse"  
        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Registro());
        }
    }
}
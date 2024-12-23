using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            await Navigation.PushAsync(new login());
        }

        // Evento para el botón "Registrarse"  
        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Registro());
        }
    }
}
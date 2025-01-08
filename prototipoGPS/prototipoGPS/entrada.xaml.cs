using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace prototipoGPS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class entrada : ContentPage
    {
        public entrada()
        {
            InitializeComponent();
        }

        private async void OninicioClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Inicio());
        }

        private void OnPerfilTapped(object sender, EventArgs e)
        {
            // Alternar la visibilidad de los botones al tocar el perfil
            BotonesPerfil.IsVisible = !BotonesPerfil.IsVisible;
        }
   

        private async void OnSalirClicked(object sender, EventArgs e)
        {
            // Borrar preferencias de inicio de sesión
            Preferences.Remove("IsLoggedIn");
            Preferences.Remove("userNombre");

            // Redirigir a la pantalla principal
            await Navigation.PushAsync(new MainPage());
        }

    }
}

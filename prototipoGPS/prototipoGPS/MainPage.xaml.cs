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
            // Aquí puedes agregar la lógica para continuar con la aplicación
            // Por ejemplo, navegar a otra página o mostrar un mensaje de bienvenida
            //await DisplayAlert("Continuar", "Has hecho clic en 'Continuar con Trace'", "OK");
            await Navigation.PushAsync(new login());


            // Ejemplo: Navegar a una nueva página (si tienes otra página definida)
            // await Navigation.PushAsync(new HomePage());
        }

        // Evento para el botón "Registrarse"
        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Aquí puedes agregar la lógica para el registro
            // Por ejemplo, abrir una página de registro o mostrar un mensaje
            //await DisplayAlert("Registro", "Has hecho clic en 'Registrarse'", "OK");
            await Navigation.PushAsync(new Registro());

            // Ejemplo: Navegar a una página de registro (si tienes una definida)
            // await Navigation.PushAsync(new RegisterPage());
        }

        

    }
}
using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json; // Asegúrate de tener Newtonsoft.Json instalado
using Xamarin.Forms;
using prototipoGPS.Modelos;
using Xamarin.Essentials;

namespace prototipoGPS
{
    public partial class Inicio : ContentPage
    {
        public Inicio()
        {
            InitializeComponent();

            // Obtener el nombre almacenado localmente en Preferences
            string userNombre = Preferences.Get("userNombre", "Usuario no encontrado");

            // Verificar si el nombre del usuario no es el valor por defecto
            if (userNombre != "Usuario no encontrado" && !string.IsNullOrEmpty(userNombre))
            {
                // Establecer el texto del Label con el nombre del usuario
                UserNameLabel.Text = $"Bienvenido, {userNombre}";

                // Establecer el texto del Entry con el nombre del usuario
                UserNameEntry.Text = userNombre;
            }
            else
            {
                // Si no se encontró el nombre, muestra un valor por defecto
                UserNameLabel.Text = "Bienvenido, Usuario";
            }

            // Mostrar el Frame con el Label
            // Mostrar el Frame con el Label
            Device.BeginInvokeOnMainThread(async () =>
            {
                // Hacer visible el Frame
                UserNameFrame.IsVisible = true;

                // Asegurarse de que el Frame esté fuera de la pantalla antes de animarlo
                UserNameFrame.TranslationX = this.Width;

                // Animar el Frame (y el Label dentro) para que se deslice desde la derecha hacia la izquierda
                await UserNameFrame.TranslateTo(0, 0, 500, Easing.CubicInOut);

                // Iniciar un temporizador para ocultar el Frame después de 5 segundos
                Device.StartTimer(TimeSpan.FromSeconds(5), () =>
                {
                    // Animar el Frame hacia la derecha antes de ocultarlo
                    UserNameFrame.TranslateTo(this.Width, 0, 500, Easing.CubicInOut);

                    // Ocultar el Frame (y el Label) después de la animación
                    UserNameFrame.IsVisible = false;
                    return false; // Detener el temporizador
                });
            });

        }

        // Método para registrar un objeto
        //private async void OnBusquedaClicked(object sender, EventArgs e)
        //{
        //    string nombreObjeto = NombreObjetoEntry.Text;

        //    // Validar que el campo no esté vacío
        //    if (string.IsNullOrEmpty(nombreObjeto))
        //    {
        //        await DisplayAlert("Error", "Por favor, ingresa el nombre del objeto.", "OK");
        //        return;
        //    }

        //    // Validar que el nombre del objeto no contenga números
        //    if (ContieneNumeros(nombreObjeto))
        //    {
        //        await DisplayAlert("Error", "El nombre del objeto no puede contener números.", "OK");
        //        return;
        //    }

        //    // Si pasa las validaciones, proceder a registrar el objeto
        //    RegistroObjeto registro = new RegistroObjeto
        //    {
        //        NombreObjeto = nombreObjeto,
        //        FechaHora = DateTime.Now
        //    };

        //    try
        //    {
        //        string json = JsonConvert.SerializeObject(registro);

        //        using (HttpClient client = new HttpClient())
        //        {
        //            string url = "http://192.168.1.49:3000/registrar-objeto"; // Cambia por tu URL real
        //            var content = new StringContent(json, Encoding.UTF8, "application/json");

        //            HttpResponseMessage response = await client.PostAsync(url, content);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                await DisplayAlert("Éxito", "Objeto registrado correctamente.", "OK");
        //            }
        //            else
        //            {
        //                await DisplayAlert("Error", "No se pudo registrar el objeto.", "OK");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
        //    }
        //}

        //// Método para validar si un texto contiene números
        //private bool ContieneNumeros(string texto)
        //{
        //    foreach (char c in texto)
        //    {
        //        if (char.IsDigit(c))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        // Método para ir a la página de registros
        private async void OnRegisterPClicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new RegistroPertenencias());
            await Navigation.PushAsync(new busqueda());

        }
    }
}

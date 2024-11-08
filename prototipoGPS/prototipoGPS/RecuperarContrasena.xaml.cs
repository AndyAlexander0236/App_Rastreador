using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace prototipoGPS
{
    public partial class RecuperarContrasena : ContentPage
    {
        public RecuperarContrasena()
        {
            InitializeComponent();
        }

        private async void OnEnviarSolicitudClicked(object sender, EventArgs e)
        {
            string correo = CorreoEntry.Text;
            if (string.IsNullOrEmpty(correo))
            {
                await DisplayAlert("Error", "Por favor ingrese su correo electrónico.", "OK");
                return;
            }

            // Llamada a tu servidor para manejar la solicitud de recuperación
            var result = await EnviarSolicitudRecuperacionAsync(correo);
            if (result)
            {
                await DisplayAlert("Éxito", "Solicitud de recuperación enviada. Revisa tu correo.", "OK");
                await Navigation.PopAsync(); // Regresa a la página anterior
            }
            else
            {
                await DisplayAlert("Error", "No se pudo enviar la solicitud. Inténtalo nuevamente.", "OK");
            }
        }

        private async Task<bool> EnviarSolicitudRecuperacionAsync(string correo)
        {
            using (var client = new HttpClient())
            {
                var data = new { Correo = correo };
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                //var response = await client.PostAsync("http://10.0.2.2:3000/recuperar", content);
                var response = await client.PostAsync("http://192.168.1.49:3000/recuperar", content); //IP de la Maquina

                return response.IsSuccessStatusCode;
            }
        }
    }
}

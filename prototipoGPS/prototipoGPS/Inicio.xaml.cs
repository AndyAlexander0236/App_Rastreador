using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;
using Xamarin.Essentials;
using prototipoGPS.Modelos;  // Necesario para usar la clase Configuracion y RegistroObjeto

namespace prototipoGPS
{
    public partial class Inicio : ContentPage
    {
        public Inicio()
        {
            InitializeComponent();
            Configuracion.CargarDesdePreferencias(); // Cargar la configuración guardada

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

        private async void OnRegisterPClicked(object sender, EventArgs e)
        {
            // Obtener el nombre del objeto
            string nombreObjeto = NombreObjetoEntry.Text;

            if (string.IsNullOrEmpty(nombreObjeto))
            {
                await Navigation.PushModalAsync(new AlertasPersonalizadas(" A Ocurrido Un Error", "Por favor ingresa el nombre de un objeto valido"));
                return;
            }

            // Crear el objeto RegistroObjeto con los datos a enviar
            var objeto = new { nombreObjeto };

            // Obtener la IP y el puerto desde la configuración
            string ip = Configuracion.Ip;
            string puerto = Configuracion.Puerto;

            // Crear la URL completa del servidor
            string url = $"http://{ip}:{puerto}/registrar-objeto";

            await RegistrarObjeto(objeto, url);
        }

        private async Task RegistrarObjeto(object objeto, string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Convertir el objeto a JSON
                    var json = JsonConvert.SerializeObject(objeto);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Enviar la solicitud POST al servidor
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        await Navigation.PushModalAsync(new AlertasPersonalizadas(" Registro Exitoso", "Objeto registrado correctamente"));
                        await Navigation.PushAsync(new busqueda());



                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        await Navigation.PushModalAsync(new AlertasPersonalizadas(" Registro Fallido", "No se pudo registrar el objeto que has ingresado intentalo nuevamente"));
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al registrar el objeto: {ex.Message}", "OK");
            }
        }


        private async void OnRegisterPerClicked(object sender, EventArgs e)
        {

            await Navigation.PushAsync(new RegistroPertenencias());
        }

    }
}

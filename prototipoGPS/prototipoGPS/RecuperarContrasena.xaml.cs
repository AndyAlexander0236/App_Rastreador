using Xamarin.Forms;
using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using prototipoGPS.Modelos;

namespace prototipoGPS
{
    public partial class RecuperarContrasena : ContentPage
    {
        private string codigoVerificacion;

        public RecuperarContrasena()
        {
            InitializeComponent();
        }

        // Evento del botón "Recibir Código"
        private async void OnRecibirCodigoClicked(object sender, EventArgs e)
        {
            string correo = CorreoEntry.Text;

            if (string.IsNullOrEmpty(correo))
            {
                await DisplayAlert("Error", "Por favor ingresa un correo electrónico válido", "OK");
                return;
            }

            // Validar si el correo está registrado en la base de datos
            bool correoValido = await ValidarCorreo(correo);

            if (!correoValido)
            {
                await DisplayAlert("Correo no encontrado", "No pudimos encontrar este correo en nuestro sistema. Verifica la dirección ingresada o regístrate si es necesario.", "OK");
                return;
            }

            // Si el correo es válido, generar un código de verificación alfanumérico de 8 caracteres
            codigoVerificacion = GenerarCodigoVerificacion();
            VerCodigoButton.IsEnabled = true;  // Habilitar el botón para ver el código
            CodigoDisplayEntry.IsVisible = true;   // Mostrar campo solo lectura para el código
            CodigoDisplayEntry.Text = ""; // Limpiar el campo de código al recibir un nuevo código
            NotificacionLabel.Text = "Código generado correctamente.";
            NotificacionLabel.IsVisible = true;
        }

        // Evento del botón "Ver Código"
        private void OnVerCodigoClicked(object sender, EventArgs e)
        {
            // Mostrar el código generado en el campo en modo solo de lectura
            CodigoDisplayEntry.Text = codigoVerificacion;
        }

        // Evento para validar el código ingresado automáticamente
        private void OnCodigoEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            // Verificar si el código ingresado es correcto
            if (e.NewTextValue == codigoVerificacion)
            {
                // Habilita el formulario para cambiar la contraseña
                FormularioCambioContrasena.IsVisible = true;
                NuevaContrasenaEntry.IsEnabled = true;
                NotificacionLabel.Text = "Código correcto. Ahora puede cambiar la contraseña.";
                NotificacionLabel.TextColor = Color.Black;
            }
            else
            {
                // Muestra un mensaje de error si el código no es correcto
                FormularioCambioContrasena.IsVisible = false;
                NotificacionLabel.Text = "El código es incorrecto. Por favor, inténtelo de nuevo.";
                NotificacionLabel.TextColor = Color.Red;
            }

            NotificacionLabel.IsVisible = true;
        }

        // Evento para cambiar la contraseña
        private async void OnActualizarContrasenaClicked(object sender, EventArgs e)
        {
            string correo = CorreoEntry.Text;
            string nuevaContrasena = NuevaContrasenaEntry.Text;

            if (string.IsNullOrEmpty(nuevaContrasena))
            {
                await DisplayAlert("Error", "Por favor ingresa una nueva contraseña.", "OK");
                return;
            }

            // Usar la clase Configuracion para obtener la IP y el puerto
            string ip = Configuracion.Ip;
            string puerto = Configuracion.Puerto;
            string baseUrl = $"http://{ip}:{puerto}/cambiar-contrasena";

            // Solicitud para cambiar la contraseña en el servidor Node.js
            var client = new HttpClient();
            var requestBody = new { correo = correo, nuevaContrasena = nuevaContrasena };
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // Construir la URL de conexión usando los valores de la clase Configuración
                string url = $"http://{ip}:{puerto}/cambiar-contrasena";
                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Actualización Exitosa", "Contraseña actualizada correctamente.", "OK");
                    await Navigation.PopAsync();  // Volver al inicio de sesión
                }
                else
                {
                    await DisplayAlert("Error", "No fue posible actualizar la contraseña. Intenta nuevamente.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Hubo un problema con la conexión al servidor.", "OK");
            }
        }

        // Método para validar el correo
        private async Task<bool> ValidarCorreo(string correo)
        {
            try
            {
                // Construir la URL con el correo como parámetro de consulta
                string url = $"http://{Configuracion.Ip}:{Configuracion.Puerto}/validar-correo?correo={correo}";

                // Crear cliente HTTP y realizar la solicitud GET
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        // Leer la respuesta JSON del servidor
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var jsonObject = JObject.Parse(jsonResponse);

                        // Verificar si el correo existe
                        return jsonObject["existe"].Value<bool>();
                    }
                    else
                    {
                        // Manejar errores HTTP (4xx, 5xx)
                        Console.WriteLine($"Error en la solicitud: {response.StatusCode}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores de conexión u otros problemas
                Console.WriteLine($"Error al validar correo: {ex.Message}");
                return false;
            }
        }

        // Método para generar el código de verificación
        private string GenerarCodigoVerificacion()
        {
            var random = new Random();
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] codigo = new char[8];

            for (int i = 0; i < 8; i++)
            {
                codigo[i] = caracteres[random.Next(caracteres.Length)];
            }

            return new string(codigo);
        }
    }
}

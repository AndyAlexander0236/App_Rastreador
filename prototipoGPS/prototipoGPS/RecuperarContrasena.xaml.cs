using Xamarin.Forms;
using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;

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
                await DisplayAlert("Error", "Por favor ingrese un correo.", "OK");
                return;
            }

            // Validar si el correo está registrado en la base de datos
            bool correoValido = await ValidarCorreo(correo);

            if (!correoValido)
            {
                await DisplayAlert("Error", "El correo no está registrado.", "OK");
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
            // Mostrar el código generado en el campo de solo lectura
            CodigoDisplayEntry.Text = codigoVerificacion;
        }

        // Evento para validar el código ingresado automáticamente
        private void OnCodigoEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            // Verificar si el código ingresado es correcto
            if (e.NewTextValue == codigoVerificacion)
            {
                // Habilitar el formulario para cambiar la contraseña
                FormularioCambioContrasena.IsVisible = true;
                NuevaContrasenaEntry.IsEnabled = true;
                NotificacionLabel.Text = "Código correcto. Ahora puede cambiar la contraseña.";
                NotificacionLabel.TextColor = Color.Green;
            }
            else
            {
                // Mostrar un mensaje de error si el código no es correcto
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
                await DisplayAlert("Error", "Por favor ingrese la nueva contraseña.", "OK");
                return;
            }

            // Solicitud para cambiar la contraseña en el servidor Node.js
            var client = new HttpClient();
            var requestBody = new { correo = correo, nuevaContrasena = nuevaContrasena };
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://192.168.1.49:3000/cambiar-contrasena", content);
                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Éxito", "La contraseña ha sido cambiada.", "OK");
                    await Navigation.PopAsync();  // Volver al inicio de sesión
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo actualizar la contraseña.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar la contraseña: " + ex.Message);
                await DisplayAlert("Error", "Ocurrió un error en la solicitud.", "OK");
            }
        }

        // Método para validar el correo en el servidor Node.js
        private async Task<bool> ValidarCorreo(string correo)
        {
            var client = new HttpClient();
            var requestBody = new { correo = correo };
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://192.168.1.49:3000/validar-correo", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    JObject result = JObject.Parse(responseContent);
                    return result["isValid"].ToObject<bool>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al validar correo: " + ex.Message);
            }

            return false;
        }


        //// Gesto para copiar el código al portapapeles
        //private async void CopiarCodigoAlPortapapeles()
        //{
        //    if (!string.IsNullOrEmpty(codigoVerificacion))
        //    {
        //        await Clipboard.SetTextAsync(codigoVerificacion);
        //        await DisplayAlert("Copiado", "El código ha sido copiado al portapapeles.", "OK");
        //    }
        //}


        // Método para generar un código de verificación alfanumérico de 8 caracteres
        private string GenerarCodigoVerificacion()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var code = new char[8];
            for (int i = 0; i < code.Length; i++)
            {
                code[i] = chars[random.Next(chars.Length)];
            }
            return new string(code);
        }
    }
}

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Essentials;
using prototipoGPS.Modelos;

namespace prototipoGPS
{
    public partial class Registro : ContentPage
    {
        private static readonly HttpClient client = new HttpClient();
        private string baseUrl = "http://192.168.1.49:3000"; //IP de mi maquina

        public Registro()
        {
            InitializeComponent();
        }


        //prueba de conexion 
        private async void TestServerConnection()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"{baseUrl}/");
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Conexión exitosa", result, "OK");
                }
                else
                {
                    await DisplayAlert("Error", $"No se pudo conectar: {response.StatusCode}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
        }


        // Método para enviar datos al servidor
        private async Task SendDataAsync(User user)
        {
            try
            {
                // Serializar el objeto de usuario a JSON
                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Enviar solicitud POST al servidor
                var response = await client.PostAsync($"{baseUrl}/register", content);

                if (response.IsSuccessStatusCode)
                {
                    // Guardar los datos localmente (como ejemplo)
                    Preferences.Set("userCorreo", user.correo);
                    Preferences.Set("userContrasena", user.contrasena);
                    Preferences.Set("userNombre", user.nombre); // Guardar el nombre también

                    // Navegar al login
                    await DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");
                    await Navigation.PushAsync(new login());
                }
                else
                {
                    // Manejar error
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Error", $"Ocurrió un error al registrar al usuario: {errorMessage}", "OK");
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Manejo de errores de conexión
                await DisplayAlert("Error de conexión", $"No se pudo conectar con el servidor: {httpEx.Message}", "OK");
            }
            catch (Exception ex)
            {
                // Manejar excepciones generales
                await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
        }

        // Evento del botón de registro
        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Obtener datos de la interfaz de usuario
            string nombre = NombreEntry.Text;
            string correo = CorreoEntry.Text;
            string contrasena = ContrasenaEntry.Text;

            // Validación de campos vacíos
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
            {
                await DisplayAlert("Error", "Por favor, complete todos los campos", "OK");
                return;
            }

            // Obtener fecha de nacimiento en formato YYYY-MM-DD
            string dia = DiaPicker.SelectedItem?.ToString();
            string mes = (MesPicker.SelectedIndex + 1).ToString("D2"); // Asegura que el mes tenga dos dígitos
            string anio = AnioPicker.SelectedItem?.ToString();

            DateTime? fecha_nacimiento = null;
            if (!string.IsNullOrEmpty(dia) && !string.IsNullOrEmpty(mes) && !string.IsNullOrEmpty(anio))
            {
                try
                {
                    string fecha_nacimiento_str = $"{anio}-{mes}-{dia}";
                    fecha_nacimiento = DateTime.Parse(fecha_nacimiento_str);
                }
                catch (FormatException)
                {
                    await DisplayAlert("Error", "La fecha de nacimiento es inválida.", "OK");
                    return;
                }
            }
            else
            {
                await DisplayAlert("Error", "Por favor, ingrese una fecha de nacimiento válida.", "OK");
                return;
            }

            // Crear un nuevo objeto User
            var user = new User
            {
                nombre = nombre,
                correo = correo,
                contrasena = contrasena,
                fecha_nacimiento = fecha_nacimiento
            };

            // Enviar los datos al servidor
            await SendDataAsync(user);
        }
    }
}

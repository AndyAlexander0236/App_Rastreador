using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
        private string baseUrl = "http://192.168.1.49:3000"; // IP de mi máquina

        public Registro()
        {
            InitializeComponent();
        }

        // Método para validar entradas
        private bool ValidarEntradas()
        {
            // Validar que el nombre no esté vacío y solo contenga letras
            if (string.IsNullOrEmpty(NombreEntry.Text) || !Regex.IsMatch(NombreEntry.Text, @"^[a-zA-Z\s]+$"))
            {
                DisplayAlert("Error", "El nombre solo puede contener letras y no debe estar vacío.", "OK");
                return false;
            }

            // Validar que el correo tenga un formato válido
            if (string.IsNullOrEmpty(CorreoEntry.Text) ||
                !Regex.IsMatch(CorreoEntry.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                DisplayAlert("Error", "Por favor, ingrese un correo electrónico válido.", "OK");
                return false;
            }

            // Validar que la contraseña no esté vacía
            if (string.IsNullOrEmpty(ContrasenaEntry.Text))
            {
                DisplayAlert("Error", "Por favor, ingrese una contraseña.", "OK");
                return false;
            }

            // Validar que se haya seleccionado una fecha completa
            if (DiaPicker.SelectedItem == null || MesPicker.SelectedItem == null || AnioPicker.SelectedItem == null)
            {
                DisplayAlert("Error", "Por favor, seleccione una fecha de nacimiento completa.", "OK");
                return false;
            }

            return true;
        }

        // Evento del botón de registro
        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Validar campos
            if (!ValidarEntradas())
                return;

            // Obtener datos de la interfaz de usuario
            string nombre = NombreEntry.Text;
            string correo = CorreoEntry.Text;
            string contrasena = ContrasenaEntry.Text;

            // Obtener fecha de nacimiento en formato YYYY-MM-DD
            string dia = DiaPicker.SelectedItem?.ToString();
            string mes = (MesPicker.SelectedIndex + 1).ToString("D2"); // Mes con dos dígitos
            string anio = AnioPicker.SelectedItem?.ToString();

            DateTime? fecha_nacimiento = null;
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

        // Método para enviar datos al servidor
        private async Task SendDataAsync(User user)
        {
            try
            {
                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{baseUrl}/register", content);

                if (response.IsSuccessStatusCode)
                {
                    Preferences.Set("userCorreo", user.correo);
                    Preferences.Set("userContrasena", user.contrasena);
                    Preferences.Set("userNombre", user.nombre);

                    await DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");
                    await Navigation.PushAsync(new login());
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Error", $"Ocurrió un error al registrar al usuario: {errorMessage}", "OK");
                }
            }
            catch (HttpRequestException httpEx)
            {
                await DisplayAlert("Error de conexión", $"No se pudo conectar con el servidor: {httpEx.Message}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
        }
    }
}

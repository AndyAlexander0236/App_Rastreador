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
                Navigation.PushModalAsync(new AlertasPersonalizadas("Validación de Nombre",
                                                                  "El nombre solo debe contener letras y no puede estar vacío. Intenta nuevamente."));
                return false;
            }

            // Validar que el correo tenga un formato válido
            if (string.IsNullOrEmpty(CorreoEntry.Text) ||
                !Regex.IsMatch(CorreoEntry.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                Navigation.PushModalAsync(new AlertasPersonalizadas("Correo Electrónico Inválido",
                                                                  "Por favor, ingresa un correo electrónico válido para continuar."));
                return false;
            }

            // Validar que la contraseña no esté vacía
            if (string.IsNullOrEmpty(ContrasenaEntry.Text))
            {
                Navigation.PushModalAsync(new AlertasPersonalizadas("Validación de Contraseña",
                                                                  "Es necesario que ingreses una contraseña. No puedes dejar este campo vacío."));
                return false;
            }

            // Validar que se haya seleccionado una fecha completa
            if (DiaPicker.SelectedItem == null || MesPicker.SelectedItem == null || AnioPicker.SelectedItem == null)
            {
                Navigation.PushModalAsync(new AlertasPersonalizadas("Fecha de Nacimiento Incompleta",
                                                                  "Selecciona una fecha de nacimiento válida y asegúrate de que sea completa."));
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
                await Navigation.PushModalAsync(new AlertasPersonalizadas("Fecha de Nacimiento Incorrecta", 
                                                                          "La fecha de nacimiento proporcionada no es válida. " +
                                                                          "Por favor, revisa y corrige el dato."));              
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

                    await Navigation.PushModalAsync(new AlertasPersonalizadas("Registro Exitoso","Te has registrado correctamente"));

                    await Navigation.PushAsync(new login());
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    await Navigation.PushModalAsync(new AlertasPersonalizadas("Error", "A ocurrido un error al registrar al usuario"));

                }
            }
            catch (HttpRequestException httpEx)
            {
                await Navigation.PushModalAsync(new AlertasPersonalizadas("Error de conexión", "No se pudo conectar con el servidor"));

            }
            catch (Exception ex)
            {
                await Navigation.PushModalAsync(new AlertasPersonalizadas("Error", $"Ocurrio un error:{ex.Message} "));

            }
        }
    }
}

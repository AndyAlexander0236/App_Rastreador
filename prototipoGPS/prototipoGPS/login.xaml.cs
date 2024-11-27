using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using prototipoGPS.Modelos;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace prototipoGPS
{
    public partial class login : ContentPage
    {
        private HttpClient client;
        private string baseUrl = "http://192.168.1.49:3000"; 
        //private string baseUrl = "http://10.0.2.2:3000"; 


        public login()
        {
            InitializeComponent();
            client = new HttpClient();
        }

        // Evento del botón de inicio de sesión
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // Obtener datos de la interfaz de usuario
            string correo = CorreoEntry.Text;
            string contrasena = ContrasenaEntry.Text;

            // Crear un objeto que se enviará al servidor con las credenciales
            var loginData = new { correo = correo, contrasena = contrasena };
            var json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // Enviar los datos al servidor para validar el login
                var response = await client.PostAsync($"{baseUrl}/login", content);

                if (response.IsSuccessStatusCode)
                {
                    // Si la respuesta es exitosa, obtener el nombre del usuario desde la respuesta
                    var result = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<User>(result); // Asumiendo que recibes los datos del usuario, incluyendo su nombre

                    // Guardar el nombre en las preferencias locales
                    Preferences.Set("userNombre", user.nombre);

                    // Mostrar mensaje de éxito y navegar a la siguiente página
                    await Navigation.PushModalAsync(new AlertasPersonalizadas("Inicio de Sesión Exitosa","Te dsamos la bienvenida"));
                    await Navigation.PushAsync(new entrada());
                }
                else
                {
                    // Si no es exitosa, mostrar mensaje de error
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    await Navigation.PushModalAsync(new AlertasPersonalizadas( "Inicio de Sesión Fallido",
                    "No pudimos iniciar sesión debido a un error. Por favor, revisa tu correo y contraseña e inténtalo nuevamente." ));


                }
            }
            catch (Exception ex)
            {
                // Manejar excepciones
                await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
        }
        // Evento para abrir la página de recuperación de contraseña
        private async void OnRecuperarContrasenaTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RecuperarContrasena());
        }



    }
}

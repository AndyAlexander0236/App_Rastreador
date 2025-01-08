using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Threading.Tasks;
using System;
using prototipoGPS.Modelos;
using System.Threading;

namespace prototipoGPS
{
    public partial class busqueda : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private bool _isTimerRunning = false;  // Bandera para controlar el temporizador
        private System.Threading.Timer _timer;  // Temporizador para el seguimiento GPS

        public busqueda()
        {
            InitializeComponent();
            LoadCurrentLocation();
        }

        // Método para obtener los datos GPS desde el servidor
        private async Task GetGPSData()
        {
            try
            {
                var url = $"http://{Configuracion.Ip}:{Configuracion.Puerto}/gps-data";
                var response = await _httpClient.GetStringAsync(url);
                Console.WriteLine("Respuesta del servidor: " + response);

                if (string.IsNullOrEmpty(response))
                {
                    throw new Exception("La respuesta del servidor está vacía.");
                }

                var gpsData = JsonConvert.DeserializeObject<GpsData>(response);
                if (gpsData != null)
                {
                    Console.WriteLine($"Datos recibidos: Latitud = {gpsData.Latitud}, Longitud = {gpsData.Longitud}");

                    if (gpsData.Latitud != 0 && gpsData.Longitud != 0)
                    {
                        // Mover el mapa a las coordenadas recibidas
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            var position = new Position(gpsData.Latitud, gpsData.Longitud);
                            googleMap.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromMiles(0.5)));

                            // Actualizar la etiqueta con las coordenadas
                            DireccionLabel.Text = $"Coordenadas: {gpsData.Latitud}, {gpsData.Longitud}";

                            // Agregar un marcador en el mapa
                            googleMap.Pins.Clear();  // Limpiar cualquier pin previo
                            googleMap.Pins.Add(new Pin
                            {
                                Label = "Ubicación del ESP32",
                                Position = position,
                                Type = PinType.Place
                            });
                        });
                    }
                    else
                    {
                        Console.WriteLine("Datos GPS no válidos.");
                        await DisplayAlert("Error", "No se pudieron obtener las coordenadas válidas del servidor.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener las coordenadas: " + ex.Message);
                await DisplayAlert("Error", "No se pudo obtener la ubicación del servidor.", "OK");
            }
        }

        // Método que carga la ubicación inicial desde el servidor
        private async void LoadCurrentLocation()
        {
            // Cargar las coordenadas del servidor al iniciar
            await GetGPSData();
        }

        // Método para iniciar el seguimiento del GPS
        private void OnBotonIniciarClicked(object sender, EventArgs e)
        {
            if (!_isTimerRunning)
            {
                _timer = new System.Threading.Timer(async _ =>
                {
                    try
                    {
                        await GetGPSData();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error en el temporizador: " + ex.Message);  // Log para ver el error
                        await DisplayAlert("Error", "Hubo un problema al obtener las coordenadas en el seguimiento.", "OK");
                    }
                }, null, 0, 5000); // Llamar cada 5 segundos

                BotonIniciar.IsEnabled = false; // Deshabilitar el botón de iniciar
                BotonParar.IsEnabled = true;  // Habilitar el botón de detener
                _isTimerRunning = true;
            }
        }

        // Método para detener el seguimiento del GPS
        private async void OnStopButtonClicked(object sender, EventArgs e)
        {
            if (_isTimerRunning)
            {
                _timer?.Change(Timeout.Infinite, Timeout.Infinite); // Detener el temporizador  
                _isTimerRunning = false;
                BotonIniciar.IsEnabled = true;  // Habilitar el botón de iniciar  
                BotonParar.IsEnabled = false; // Deshabilitar el botón de detener  

                // Mostrar el mensaje de felicitaciones  
                await Navigation.PushModalAsync(new AlertasPersonalizadas("Deteniendo Busqueda", "Felicidades, has encontrado tu pertenencia!"));

            }
        }

        // Método para enviar el comando de sonido al servidor
        private async void OnPlaySoundButtonClicked(object sender, EventArgs e)
        {
            await SendSoundCommandToESP32Async();
        }

        // Función para enviar el comando de sonido al servidor
        private async Task SendSoundCommandToESP32Async()
        {
            try
            {
                string ip = Configuracion.Ip; // IP configurada en Configuración.cs
                string puerto = Configuracion.Puerto; // Puerto configurado en Configuración.cs
                string url = $"http://{ip}:{puerto}/play-sound"; // Ruta del servidor que activará el sonido en el ESP32
                //string url = $"http://192.168.1.37:80/play-sound"; // Aquí puedes dejar la URL del ESP32 para enviar el comando

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    await Navigation.PushModalAsync(new AlertasPersonalizadas("Sonido Enviado", "Se ha enviado correctamente el sonido al dispositivo"));

                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Error", $"No se pudo enviar el sonido correctamente: {responseContent}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al enviar el comando: {ex.Message}", "OK");
            }
        }
    }
}

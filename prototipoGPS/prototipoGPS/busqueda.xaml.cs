using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using prototipoGPS.Modelos;

namespace prototipoGPS
{
    public partial class busqueda : ContentPage
    {
        private static readonly HttpClient client = new HttpClient();
        private bool isFetchingData = false;

        public busqueda()
        {
            InitializeComponent();
            ShowCurrentLocationOnMap(); // Mostrar ubicación actual al iniciar
        }

        // Método para obtener los datos GPS desde el servidor
        private async Task FetchGPSDataAsync()
        {
            string ip = Configuracion.Ip; // Asegúrate de configurar la IP en Configuracion.cs
            string puerto = Configuracion.Puerto; // Puerto del servidor
            string url = $"http://{ip}:{puerto}/gps-data";

            try
            {
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Error", "No se pudieron obtener los datos GPS.", "OK");
                    return;
                }

                var result = await response.Content.ReadAsStringAsync();
                var gpsData = DeserializeGPSData(result);

                if (gpsData == null || !gpsData.Any())
                {
                    await DisplayAlert("Sin Datos", "No se encontraron datos GPS válidos.", "OK");
                    return;
                }

                UpdateUIWithGPSData(gpsData);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al obtener datos GPS: {ex.Message}", "OK");
            }
        }

        // Deserialización de JSON a lista de datos GPS
        private List<GpsData> DeserializeGPSData(string json)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<List<GpsData>>(json);
                return data;
            }
            catch (JsonException ex)
            {
                DisplayAlert("Error", $"Error al parsear datos GPS: {ex.Message}", "OK");
                return new List<GpsData>();
            }
        }

        // Actualiza la interfaz con los datos obtenidos
        private void UpdateUIWithGPSData(List<GpsData> gpsData)
        {
            if (gpsData == null || !gpsData.Any())
            {
                DisplayAlert("Sin datos", "No se encontraron registros GPS disponibles.", "OK");
                return;
            }

            // Ordenar datos por fecha descendente
            gpsData = gpsData.OrderByDescending(d => d.Timestamp).ToList();

            // Actualizar el ListView con los datos
            gpsListView.ItemsSource = gpsData;

            // Obtener la primera posición para centrar el mapa
            var firstData = gpsData.First();
            var position = new Position(firstData.Latitud, firstData.Longitud);

            googleMap.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(1)));

            // Configurar y agregar el pin al mapa
            var pin = new Pin
            {
                Type = PinType.Place,
                Position = position,
                Label = "Mochila de rastreo",
                Address = $"Lat: {firstData.Latitud}, Lon: {firstData.Longitud}"
            };

            googleMap.Pins.Clear();
            googleMap.Pins.Add(pin);
        }

        // Mostrar la ubicación actual del dispositivo
        private async void ShowCurrentLocationOnMap()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location == null)
                {
                    location = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Medium,
                        Timeout = TimeSpan.FromSeconds(10)
                    });
                }

                if (location != null)
                {
                    var position = new Position(location.Latitude, location.Longitude);
                    googleMap.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(1)));

                    var pin = new Pin
                    {
                        Type = PinType.Place,
                        Position = position,
                        Label = "Ubicación Actual",
                        Address = $"Lat: {location.Latitude}, Lon: {location.Longitude}"
                    };

                    googleMap.Pins.Add(pin);
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo obtener la ubicación actual.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al obtener ubicación: {ex.Message}", "OK");
            }
        }

        // Iniciar la obtención periódica de datos
        private async void StartFetchingData()
        {
            if (isFetchingData) return;

            isFetchingData = true;
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;

            while (isFetchingData)
            {
                await FetchGPSDataAsync();
                await Task.Delay(5000);
            }
        }

        // Detener la obtención de datos
        private void StopFetchingData()
        {
            isFetchingData = false;
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
        }

        // Evento botón Iniciar
        private void OnStartButtonClicked(object sender, EventArgs e)
        {
            StartFetchingData();
        }

        // Evento botón Detener
        private void OnStopButtonClicked(object sender, EventArgs e)
        {
            StopFetchingData();
        }

        // Evento que se dispara cuando se hace clic en el botón "Mandar Sonido"
        private async void OnPlaySoundButtonClicked(object sender, EventArgs e)
        {
            // Enviar comando al servidor para activar el sonido en el ESP32
            await SendSoundCommandToESP32Async();
        }

        // Función para enviar el comando de sonido al servidor
        private async Task SendSoundCommandToESP32Async()
        {
            try
            {
                string ip = Configuracion.Ip; // IP configurada en Configuración.cs
                string puerto = Configuracion.Puerto; // Puerto configurado en Configuración.cs
                //string url = $"http://{ip}:{puerto}/play-sound"; // Ruta del servidor que activará el sonido en el ESP32
                string url = $"http://192.168.1.37:80/play-sound";

                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Éxito", "Comando de sonido enviado al ESP32.", "OK");
                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Error", $"No se pudo enviar el comando de sonido al ESP32. Respuesta: {responseContent}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al enviar el comando: {ex.Message}", "OK");
            }
        }
    }


}

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using prototipoGPS.Modelos;

namespace prototipoGPS
{
    public partial class RegistroPertenencias : ContentPage
    {
        public RegistroPertenencias()
        {
            InitializeComponent();
            CargarRegistros(); // Llamar al método para cargar los registros al inicio
        }

        // Método para obtener registros desde el servidor
        private async void CargarRegistros()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Obtener la IP y el puerto desde la configuración
                    string ip = Configuracion.Ip;
                    string puerto = Configuracion.Puerto;

                    // Crear la URL completa del servidor
                    string url = $"http://{ip}:{puerto}/obtener-registros";

                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var registros = JsonConvert.DeserializeObject<List<RegistroObjeto>>(content);

                        // Verificar los registros recibidos
                        foreach (var registro in registros)
                        {
                            Console.WriteLine($"NombreObjeto: {registro.NombreObjeto}, FechaHora: {registro.FechaHora}");
                        }

                        // Enlazar los registros al ListView
                        ListaRegistros.ItemsSource = registros;
                    }
                    else
                    {
                        await Navigation.PushModalAsync(new AlertasPersonalizadas(" A Ocurrido Un Error", "No se a podido obtener los registros"));
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
        }
    }
}

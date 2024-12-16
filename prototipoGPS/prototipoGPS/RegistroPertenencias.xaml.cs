using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace prototipoGPS
{
    public partial class RegistroPertenencias : ContentPage
    {
        public RegistroPertenencias()
        {
            InitializeComponent();
            // CargarRegistros();
        }

        // Método para obtener registros desde el servidor
    //    private async void CargarRegistros()
    //    {
    //        try
    //        {
    //            using (HttpClient client = new HttpClient())
    //            {
    //                string url = "http:192.168.1.49:3000/obtener-registros"; // Cambia por tu URL real
    //                HttpResponseMessage response = await client.GetAsync(url);

    //                if (response.IsSuccessStatusCode)
    //                {
    //                    string content = await response.Content.ReadAsStringAsync();
    //                    var registros = JsonConvert.DeserializeObject<List<RegistroObjeto>>(content);
    //                    ListaRegistros.ItemsSource = registros;
    //                }
    //                else
    //                {
    //                    await DisplayAlert("Error", "No se pudieron obtener los registros.", "OK");
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
    //        }
    //    }
    //}

    //public class RegistroObjeto
    //{
    //    public string NombreObjeto { get; set; }
    //    public DateTime FechaHora { get; set; }
    }
}

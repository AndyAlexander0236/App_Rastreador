using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace prototipoGPS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AlertasPersonalizadas : ContentPage
    {
        public AlertasPersonalizadas(string title, string message)
        {
            InitializeComponent(); // Inicializa los componentes de la página
            TitleLabel.Text = title; // Establece el título
            MessageLabel.Text = message; // Establece el mensaje
        }

        private async void OnCloseButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(); // Cierra la ventana modal
        }
    }
}
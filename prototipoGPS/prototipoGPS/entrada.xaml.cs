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
	public partial class entrada : ContentPage
	{
		public entrada()
		{
			InitializeComponent();
		}

		private async void OninicioClicked(object sender, EventArgs e)
		{
			
			await Navigation.PushAsync(new Inicio());
		}
	}
}
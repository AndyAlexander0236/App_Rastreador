using System;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace prototipoGPS
{
    public partial class Inicio : ContentPage
    {
        public Inicio()
        {
            InitializeComponent();

            // Obtener el nombre almacenado localmente en Preferences
            string userNombre = Preferences.Get("userNombre", "Usuario no encontrado");

            // Verificar si el nombre del usuario no es el valor por defecto
            if (userNombre != "Usuario no encontrado" && !string.IsNullOrEmpty(userNombre))
            {
                // Establecer el texto del Label con el nombre del usuario
                UserNameLabel.Text = $"Bienvenido, {userNombre}";

                // Establecer el texto del Entry con el nombre del usuario
                UserNameEntry.Text = userNombre;
            }
            else
            {
                // Si no se encontró el nombre, muestra un valor por defecto
                UserNameLabel.Text = "Bienvenido, Usuario";
            }

            // Mostrar el Frame con el Label
            // Mostrar el Frame con el Label
            Device.BeginInvokeOnMainThread(async () =>
            {
                // Hacer visible el Frame
                UserNameFrame.IsVisible = true;

                // Asegurarse de que el Frame esté fuera de la pantalla antes de animarlo
                UserNameFrame.TranslationX = this.Width;

                // Animar el Frame (y el Label dentro) para que se deslice desde la derecha hacia la izquierda
                await UserNameFrame.TranslateTo(0, 0, 500, Easing.CubicInOut);

                // Iniciar un temporizador para ocultar el Frame después de 5 segundos
                Device.StartTimer(TimeSpan.FromSeconds(5), () =>
                {
                    // Animar el Frame hacia la derecha antes de ocultarlo
                    UserNameFrame.TranslateTo(this.Width, 0, 500, Easing.CubicInOut);

                    // Ocultar el Frame (y el Label) después de la animación
                    UserNameFrame.IsVisible = false;
                    return false; // Detener el temporizador
                });
            });
        }
    }
}
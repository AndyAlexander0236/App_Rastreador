using Xamarin.Essentials;

namespace prototipoGPS.Modelos
{
    // Clase estática para manejar la configuración global
    public static class Configuracion
    {
        private static string _ip = "192.168.1.49"; // IP inicial
        private static string _puerto = "3000"; // Puerto inicial

        public static string Ip
        {
            get => _ip;
            set
            {
                _ip = value;
                ActualizarPreferencias();
            }
        }

        public static string Puerto
        {
            get => _puerto;
            set
            {
                _puerto = value;
                ActualizarPreferencias();
            }
        }

        private static void ActualizarPreferencias()
        {
            // Almacenar la IP y el puerto en las preferencias
            Preferences.Set("ServerIp", _ip);
            Preferences.Set("ServerPort", _puerto);
        }

        // Cargar las preferencias desde el almacenamiento local
        public static void CargarDesdePreferencias()
        {
            _ip = Preferences.Get("ServerIp", "192.168.1.49"); // IP predeterminada
            _puerto = Preferences.Get("ServerPort", "3000");   // Puerto predeterminado
        }
    }
}

using System;

namespace prototipoGPS.Modelos
{
    public class User
    {
        public string nombre { get; set; }
        public string correo { get; set; }
        public string contrasena { get; set; }
        public DateTime? fecha_nacimiento { get; set; }
    }
}
using System;
using System.Globalization;
using Xamarin.Forms;

namespace prototipoGPS
{
    public class InverseBoolConverter : IValueConverter
    {
        // Convertir la longitud (int) a un booleano
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is int length && length == 0; // Devuelve true si la longitud es 0 (sin texto)
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // No es necesario, pero se podría implementar si es necesario revertir la conversión
            return value;
        }
    }
}

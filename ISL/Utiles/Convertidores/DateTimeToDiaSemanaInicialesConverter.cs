using ISL.Servicios;
using System.Globalization;

namespace ISL.Utiles.Convertidores
{
    public class DateTimeToDiaSemanaInicialesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FechaServicio fechaServicio = new();
            return value is not null ? fechaServicio.DiaSemanaIniciales((DateTime)value) : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

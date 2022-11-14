using ISL.Servicios;
using System.Globalization;

namespace ISL.Utiles.Convertidores;

public class DateTimeToDiaSemanaConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return string.Empty;
        }
        FechaServicio fechaServicio = new();
        return fechaServicio.DiaSemana((DateTime)value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

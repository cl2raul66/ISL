using System.Globalization;

namespace ISL.Utiles.Convertidores;

public class TimeOnlyToStringViewConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is not null ? ((TimeOnly)value).ToString("t") : string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

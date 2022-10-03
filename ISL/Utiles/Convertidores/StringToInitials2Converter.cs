using System.Globalization;

namespace ISL.Utiles.Convertidores
{
    public class StringToInitials2Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? valueString = value as string;
            return !string.IsNullOrEmpty(valueString) ? valueString[..2] : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

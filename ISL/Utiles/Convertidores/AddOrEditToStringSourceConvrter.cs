using System.Globalization;

namespace ISL.Utiles.Convertidores;

public class AddOrEditToStringSourceConvrter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return "list_add.png";
        }
        return string.IsNullOrEmpty((string)value) ? "list_add.png" : "document_edit.png";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

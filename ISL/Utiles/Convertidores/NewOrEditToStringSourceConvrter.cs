using System.Globalization;

namespace ISL.Utiles.Convertidores;

public class NewOrEditToStringSourceConvrter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is not null ? (string)value != "No hay actividades" ? "document_edit.png" : "document_new.png" : string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

using ISL.Utiles.Enumeradores;
using System.Globalization;

namespace ISL.Utiles.Convertidores;

public class MensajeTipoToImagenConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string resul = null;

        switch ((MensajeTipo)value)
        {
            case MensajeTipo.Precaucion:
                resul = "dialog_warning.png";
                break;
            case MensajeTipo.Informacion:
                resul = "dialog_information.png";
                break;
        }

        return resul;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

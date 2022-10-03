using ISL.Utiles.Enumeradores;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISL.Utiles.Convertidores;

public class MensajeTipoToBacgrundColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var resul = Color.Parse("Black");

        if (value is null)
        {
            return resul;
        }

        switch ((MensajeTipo)value)
        {
            case MensajeTipo.Precaucion:
                resul = (App.Current.Resources["BgPrecaucion"] as SolidColorBrush).Color;
                break;
            case MensajeTipo.Informacion:
                resul = (App.Current.Resources["BgInformacion"] as SolidColorBrush).Color;
                break;
        }

        return resul;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

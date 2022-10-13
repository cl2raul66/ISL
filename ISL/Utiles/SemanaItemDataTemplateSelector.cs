using ISL.Modelos;
using System;

namespace ISL.Utiles;

public class SemanaItemDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate ValidTemplate { get; set; }
    public DataTemplate InvalidTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        //bool tieneTexto = !string.IsNullOrEmpty((item as SemanaItem).Texto);
        //bool tieneHorario = (item as SemanaItem).Horario is not null;
        bool resul = !string.IsNullOrEmpty((item as SemanaItem).Texto) ? true : (item as SemanaItem).Horario is not null ? true : false;

        return resul ? ValidTemplate : InvalidTemplate;
    }
}

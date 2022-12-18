using System.Globalization;

namespace ISL.Servicios;

public interface IFechaServicio
{
    DayOfWeek DowHoy { get; }
    DateTime Hoy { get; }
    int NoSemanaDelAnio { get; }
    DateTime PrimerDia { get; }
    DateTime UltimoDia { get; }
    string DiaSemana(DateTime fecha);
    string DiaSemanaIniciales(DateTime fecha);
}

public class FechaServicio : IFechaServicio
{
    private readonly DateTimeFormatInfo dtfi;

    private readonly GregorianCalendar gc = new(GregorianCalendarTypes.USEnglish);

    public FechaServicio()
    {
        dtfi = new()
        {
            FirstDayOfWeek = DayOfWeek.Monday,
            DateSeparator = "/",
            FullDateTimePattern = "dddd, dd/MM/yyyy",
            ShortDatePattern = "dd/MM/yyyy",
            DayNames = new[] { "Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado" },
            AbbreviatedDayNames = new[] { "Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa" },
            ShortestDayNames = new[] { "Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab" },
        };
    }

    public DateTime Hoy => DateTime.Now;
    public DayOfWeek DowHoy => Hoy.DayOfWeek;
    public int NoSemanaDelAnio => gc.GetWeekOfYear(Hoy, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
    public DateTime PrimerDia
    {
        get
        {
            int d = (int)DowHoy;
            if (d == 0)
                return gc.AddDays(Hoy, -6);
            else if (d == 1)
                return Hoy;
            else
                return gc.AddDays(Hoy, -1 * (d - 1));
        }
    }
    public DateTime UltimoDia => gc.AddDays(Hoy, (int)DowHoy == 0 ? 0 : 7 - (int)DowHoy);
    public string DiaSemana(DateTime fecha) => dtfi.GetDayName(fecha.DayOfWeek);
    public string DiaSemanaIniciales(DateTime fecha) => dtfi.GetAbbreviatedDayName(fecha.DayOfWeek);
}

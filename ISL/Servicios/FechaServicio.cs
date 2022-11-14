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
    public DateTime PrimerDia => gc.AddDays(Hoy, (int)DowHoy == 0 ? -6 : (int)DowHoy == 1 ? 0 : -1 * (int)DowHoy);
    public DateTime UltimoDia => gc.AddDays(Hoy, (int)DowHoy == 0 ? 0 : 7 - (int)DowHoy);
    public string DiaSemana(DateTime fecha) => dtfi.GetAbbreviatedDayName(fecha.DayOfWeek);
}

using System.Globalization;

namespace ISL.Servicios;

public interface IFechaServicio
{
    Dictionary<string, DateTime> DiasFechas { get; }
    DayOfWeek DowHoy { get; }
    DateTime Hoy { get; }
    DateOnly HoyFecha { get; }
    DateTime PrimerDia { get; }
    DateTime UltimoDia { get; }

    DateTime DateStringToDateTime(string fecha);
    string DateTimeToDateString(DateTime fecha);
    string DateTimeToTimeString(DateTime hora);
    string DiaSemana(DateOnly fecha);
    string DiaSemana(DateTime fecha);
    string DiaSemanaIniciales(DateOnly fecha);
    string DiaSemanaIniciales(DateTime fecha);
    int NoSemanaDelAnio(DateTime? fecha = null);
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

    public DateTime Hoy
    {
        get
        {
            var hoy = DateTime.Now;
            return new(hoy.Year, hoy.Month, hoy.Day);
        }
    }
    public DateOnly HoyFecha => DateOnly.FromDateTime(Hoy);
    public DayOfWeek DowHoy => Hoy.DayOfWeek;
    public int NoSemanaDelAnio(DateTime? fecha = null) => fecha switch
    {
        null => gc.GetWeekOfYear(Hoy, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday),
        _ => gc.GetWeekOfYear(fecha.Value, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday)
    };
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
    public string DiaSemana(DateOnly fecha) => dtfi.GetDayName(fecha.DayOfWeek);
    public string DiaSemanaIniciales(DateTime fecha) => dtfi.GetAbbreviatedDayName(fecha.DayOfWeek);
    public string DiaSemanaIniciales(DateOnly fecha) => dtfi.GetAbbreviatedDayName(fecha.DayOfWeek);
    public Dictionary<string, DateTime> DiasFechas => new() {
        { DiaSemanaIniciales(PrimerDia), PrimerDia },
        { DiaSemanaIniciales(PrimerDia.AddDays(1)), PrimerDia.AddDays(1) },
        { DiaSemanaIniciales(PrimerDia.AddDays(2)), PrimerDia.AddDays(2) },
        { DiaSemanaIniciales(PrimerDia.AddDays(3)), PrimerDia.AddDays(3) },
        { DiaSemanaIniciales(PrimerDia.AddDays(4)), PrimerDia.AddDays(4) },
        { DiaSemanaIniciales(PrimerDia.AddDays(5)), PrimerDia.AddDays(5) },
        { DiaSemanaIniciales(UltimoDia), UltimoDia }
    };

    #region Útiles
    public string DateTimeToDateString(DateTime fecha) => fecha.ToString("yyyyMMdd");
    public string DateTimeToTimeString(DateTime hora) => hora.ToString("t");
    public DateTime DateStringToDateTime(string fecha) => new(int.Parse(fecha[..4]), int.Parse(fecha[4..6]), int.Parse(fecha[6..]));
    //public string HoraToString(DateTime horario) => horario.ToString("t");
    //public string ToDateStringView(DateTime fecha) => fecha.ToString("dd/MM/yyyy");
    #endregion
}
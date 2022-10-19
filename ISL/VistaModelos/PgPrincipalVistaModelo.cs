using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ISL.Modelos;
using ISL.Servicios;
using ISL.Utiles.Enumeradores;
using ISL.Vistas;
using System.Collections.ObjectModel;
using System.Globalization;

namespace ISL.VistaModelos;

public partial class PgPrincipalVistaModelo : ObservableObject
{
    private readonly ITransitoriaBdServicio TransitoriaBd = MauiProgram.CreateMauiApp().Services.GetService<ITransitoriaBdServicio>();

    private DateTimeFormatInfo dtfi = new()
    {
        FirstDayOfWeek = DayOfWeek.Monday,
        DateSeparator = "/",
        FullDateTimePattern = "dddd, dd/MM/yyyy",
        ShortDatePattern = "dd/MM/yyyy",
        DayNames = new[] { "Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado" },
        AbbreviatedDayNames = new[] { "Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa" },
        ShortestDayNames = new[] { "Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab" },
    };
    private DateTime hoy = DateTime.Now;
    private DayOfWeek dow;
    private GregorianCalendar gc = new(GregorianCalendarTypes.USEnglish);
    private int NoSemanaDelAnio = 0;

    public PgPrincipalVistaModelo()
    {
        dow = hoy.DayOfWeek;
        NoSemanaDelAnio = gc.GetWeekOfYear(hoy, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

        NombreUsuario = Preferences.Get(nameof(NombreUsuario), string.Empty);
        SemanaActual = $"Semana: {NoSemanaDelAnio} del {gc.AddDays(hoy, (int)dow == 0 ? -6 : (int)dow == 1 ? 0 : -1 * (int)dow).ToShortDateString()} al {gc.AddDays(hoy, (int)dow == 0 ? 0 : 7 - (int)dow).ToShortDateString()}";
        FechaHoy = hoy.ToString("D");

        if (EnableGoTo)
        {
            SetCurrentSemana();
            SelectedSemanaItem = CurrentSemana[(int)dow == 0 ? 6 : (int)dow - 1];
        }
        else
        {
            ComprobarNombreUsuario();
        }

        if (!TransitoriaBd.ExisteBd)
        {
            Notificaciones.Add(new("No existen datos, en Ajustes configure los requeridos.", MensajeTipo.Informacion));
        }
    }

    [ObservableProperty]
    private ObservableCollection<SemanaItem> currentSemana;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EnableAgregarActividad))]
    private SemanaItem selectedSemanaItem;

    [ObservableProperty]
    private ObservableCollection<MensajeItem> notificaciones = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ComprobarNombreUsuarioCommand))]
    private string nombreUsuario;

    [ObservableProperty]
    private string fechaHoy = string.Empty;

    [ObservableProperty]
    private string semanaActual = string.Empty;

    private bool EnableGoTo => !string.IsNullOrEmpty(NombreUsuario);
    public bool EnableAgregarActividad => SelectedSemanaItem is not null;

    [RelayCommand(CanExecute = nameof(EnableGoTo))]
    private async Task GoToQrCode()
    {
        if (EnableGoTo)
        {
            await Shell.Current.GoToAsync(nameof(PgQrCode),true);
        }
    }

    [RelayCommand]
    private Task GoToAjustes() => Shell.Current.GoToAsync(nameof(PgAjustes),true);

    [RelayCommand(CanExecute = nameof(EnableAgregarActividad))]
    public async Task GoToAgregarActividad()
    {
        if (EnableAgregarActividad)
        {
            await Shell.Current.GoToAsync(nameof(PgAgregarActividad), true);
        }
    }

    private void SetCurrentSemana()
    {
        var datos = TransitoriaBd.GetLaboresFromExpediente(NoSemanaDelAnio);
        if (datos.Any())
        {
            foreach (var item in datos)
            {
                int c = item.Value.Adtividades.Count;
                ValueTuple<DateTime?, DateTime?> h = new(item.Value.Inicio, item.Value.Fin);
                string t = h.Item1 is null ? "No laborado" : c == 0 ? "No hay labores realizadas" : item.Value.Adtividades.Count > 1 ? $"{item.Value.Adtividades.Count} Labores realizadas" : $"{item.Value.Adtividades.Count} Labor realizada";
                CurrentSemana.Add(new() { NombreDia = item.Key, Texto = t, Horario = new(h.Item1?.ToString("HH:mm") ?? string.Empty, h.Item2?.ToString("HH:mm") ?? string.Empty) });
            }
        }
        else
        {
            CurrentSemana = new() {
                new SemanaItem(){ NombreDia=dtfi.GetAbbreviatedDayName(DayOfWeek.Monday)},
                new SemanaItem(){ NombreDia=dtfi.GetAbbreviatedDayName(DayOfWeek.Tuesday)},
                new SemanaItem(){ NombreDia=dtfi.GetAbbreviatedDayName(DayOfWeek.Wednesday),Texto="12 Labores realizadas", Horario=new("07:00 AM","05:00 AM")},
                new SemanaItem(){ NombreDia=dtfi.GetAbbreviatedDayName(DayOfWeek.Thursday),Horario=new("07:00 AM",null)},
                new SemanaItem(){ NombreDia=dtfi.GetAbbreviatedDayName(DayOfWeek.Friday)},
                new SemanaItem(){ NombreDia=dtfi.GetAbbreviatedDayName(DayOfWeek.Saturday)},
                new SemanaItem(){ NombreDia=dtfi.GetAbbreviatedDayName(DayOfWeek.Sunday)},
            };
        }
    }

    #region extra
    [RelayCommand]
    public async Task ComprobarNombreUsuario()
    {
        if (!EnableGoTo)
        {
            Notificaciones.Add(new("No hay usuario registrado, en Ajustes puede solucionar este problema.", MensajeTipo.Informacion));
        }
        await Task.CompletedTask;
    }
    #endregion
}

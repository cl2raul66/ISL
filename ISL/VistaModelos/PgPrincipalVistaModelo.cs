using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ISL.Modelos;
using ISL.Servicios;
using ISL.Utiles.Enumeradores;
using ISL.Vistas;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
        SemanaActual = $"Semana: {NoSemanaDelAnio} del {gc.AddDays(hoy, (int)dow == 0 ? -6 : -1 * (int)dow).ToShortDateString()} al {gc.AddDays(hoy, (int)dow == 0 ? 0 : 7 - (int)dow).ToShortDateString()}";
        FechaHoy = hoy.ToString("D");

        if (!EnableGoTo)
        {
            Notificaciones.Add(new("No hay usuario registrado, en Ajustes puede solucionar este problema.", MensajeTipo.Informacion));
        }

        if (TransitoriaBd.ExisteBd)
        {
            SetCurrentSemana();
            Notificaciones.Add(new("No existen datos, en Ajustes configure los requeridos.", MensajeTipo.Informacion));
        }
    }

    [ObservableProperty]
    private ObservableCollection<SemanaItem> currentSemana;

    [ObservableProperty]
    private List<MensajeItem> notificaciones = new();

    [ObservableProperty]
    private string nombreUsuario = string.Empty;

    [ObservableProperty]
    private string fechaHoy = string.Empty;

    [ObservableProperty]
    private string semanaActual = string.Empty;

    private bool EnableGoTo => !string.IsNullOrEmpty(NombreUsuario);

    [RelayCommand(CanExecute = nameof(EnableGoTo))]
    private async Task GoToQrCode()
    {
        if (EnableGoTo)
        {
            await Shell.Current.GoToAsync(nameof(PgQrCode));
        }
    }

    [RelayCommand]
    private Task GoToAjustes() => Shell.Current.GoToAsync($"nameof(PgAjustes)?NombreUsuario={NombreUsuario}");

    [RelayCommand(CanExecute = nameof(EnableGoTo))]
    public async Task GoToAgregarActividad()
    {
        if (EnableGoTo)
        {
            await Shell.Current.GoToAsync(nameof(PgAgregarActividad));
        }
    }

    private void SetCurrentSemana()
    {
        foreach (var item in TransitoriaBd.GetLaboresFromExpediente(NoSemanaDelAnio))
        {
            int c = item.Value.Adtividades.Count;
            ValueTuple<DateTime?, DateTime?> h = new(item.Value.Inicio, item.Value.Fin);
            string t = h.Item1 is null ? "No laborado" : c == 0 ? "No hay labores realizadas" : item.Value.Adtividades.Count > 1 ? $"{item.Value.Adtividades.Count} Labores realizadas" : $"{item.Value.Adtividades.Count} Labor realizada";
            CurrentSemana.Add(new() { NombreDia = item.Key, Texto = t, Horario = new(h.Item1?.ToString("HH:mm") ?? string.Empty, h.Item2?.ToString("HH:mm") ?? string.Empty) });
        }
        //CurrentSemana = new() {
        //    new SemanaItem(){ NombreDia=dtfi.GetAbbreviatedDayName(DayOfWeek.Monday), Texto="12 Tareas realizadas"},
        //    new SemanaItem(){ NombreDia=dtfi.GetAbbreviatedDayName(DayOfWeek.Tuesday), Texto="Hola Martes"},
        //    new SemanaItem(){ NombreDia=dtfi.GetAbbreviatedDayName(DayOfWeek.Wednesday), Texto="Hola Miércoles"},
        //    new SemanaItem(){ NombreDia=dtfi.GetAbbreviatedDayName(DayOfWeek.Thursday), Texto="Hola Jueves"},
        //    new SemanaItem(){ NombreDia=dtfi.GetAbbreviatedDayName(DayOfWeek.Friday), Texto="Hola Viernes"},
        //    new SemanaItem(){ NombreDia=dtfi.GetAbbreviatedDayName(DayOfWeek.Saturday), Texto="Hola Sábado"},
        //    new SemanaItem(){ NombreDia=dtfi.GetAbbreviatedDayName(DayOfWeek.Sunday), Texto="Hola Domingo"},
        //};
    }
}

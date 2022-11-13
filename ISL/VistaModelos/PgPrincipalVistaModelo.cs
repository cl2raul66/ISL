using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Linq;
using ISL.Modelos;
using ISL.Servicios;
using ISL.Utiles.Enumeradores;
using ISL.Vistas;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace ISL.VistaModelos;

[QueryProperty(nameof(NombreUsuario), nameof(NombreUsuario))]
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

        SemanaActual = $"Semana: {NoSemanaDelAnio} del {gc.AddDays(hoy, (int)dow == 0 ? -6 : (int)dow == 1 ? 0 : -1 * (int)dow).ToShortDateString()} al {gc.AddDays(hoy, (int)dow == 0 ? 0 : 7 - (int)dow).ToShortDateString()}";
        FechaHoy = hoy.ToString("D");

        NombreUsuario = Preferences.Get(nameof(NombreUsuario), string.Empty);
        if (!TransitoriaBd.ExisteDatos && !EnableGoTo)
        {
            Notificaciones.Add(new(0, "Sea bienvenido a Informe Semanal de Labores (ISL), ingrese los datos requeridos para poder usar esta aplicación. Gracias.", MensajeTipo.Informacion));
        }

        if (!TransitoriaBd.ExisteDatos)
        {
            Notificaciones.Add(new(1, "No existen datos, en Ajustes configure los requeridos.", MensajeTipo.Precaucion));
        }

        if (!string.IsNullOrEmpty(NombreUsuario))
        {
            SetCurrentSemana();
        }
    }

    [ObservableProperty]
    private ObservableCollection<SemanaItem> currentSemana;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EnableAgregarActividad))]
    private SemanaItem selectedSemanaItem;

    [ObservableProperty]
    private ObservableCollection<MensajeItem> notificaciones = new();

    private string nombreUsuario;
    public string NombreUsuario
    {
        get => nombreUsuario;
        set
        {
            if (SetProperty(ref nombreUsuario, value))
            {
                ComprobarNombreUsuario();
                if (string.IsNullOrEmpty(value))
                {
                    NombreUsuario = Preferences.Get(nameof(NombreUsuario), string.Empty);
                }

            }
        }
    }

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
            await Shell.Current.GoToAsync(nameof(PgQrCode), true);
        }
    }

    [RelayCommand]
    private async Task GoToAjustes() => await Shell.Current.GoToAsync(nameof(PgAjustes), true, new Dictionary<string, object>() { { nameof(NombreUsuario), NombreUsuario } });

    [RelayCommand(CanExecute = nameof(EnableAgregarActividad))]
    public async Task GoToAgregarActividad()
    {
        if (EnableAgregarActividad)
        {
            await Shell.Current.GoToAsync(nameof(PgAgregarActividad), true, new Dictionary<string, object>() { { nameof(SelectedSemanaItem), SelectedSemanaItem } });
        }
    }

    [RelayCommand]
    public async Task GoToObservaciones()
    {
        if (EnableAgregarActividad)
        {
            await Shell.Current.GoToAsync(nameof(PgModObservaciones), true);
        }
    }

    [RelayCommand]
    public Task VerObciones(string page) => page switch
    {
        "PgQrCode" => GoToQrCode(),
        "PgAjustes" => GoToAjustes(),
        _ => Task.CompletedTask
    };

    [RelayCommand]
    private async Task CompartirPlantillaManual()
    {
        var fileCache = Path.Combine(FileSystem.Current.CacheDirectory, "Modelo_ISL.docx");
        var doc = await FileSystem.Current.OpenAppPackageFileAsync("Plantilla_ISL_Manual.docx");
        FileStream fs = File.OpenWrite(fileCache);
        await doc.CopyToAsync(fs);
        fs.Close();

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Modelo ISL",
            File = new ShareFile(fileCache)
        });
    }

    private void SetCurrentSemana()
    {
        var datos = TransitoriaBd.GetExpediente?.Labores;
        if (datos?.Any() ?? false)
        {
            foreach (var item in datos)
            {
                int c = 0;
                ValueTuple<DateTime?, DateTime?> h = new(null,null);
                foreach (var ad in item.Value)
                {
                    c = ad.Adtividades.Count;
                    h = new(ad.Inicio, ad.Fin);
                    
                }
                string t = h.Item1 is null ? "No laborado" : c == 0 ? "No hay labores realizadas" : c > 1 ? $"{c} Labores realizadas" : $"{c} Labor realizada";

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

        SelectedSemanaItem = CurrentSemana[(int)dow == 0 ? 6 : (int)dow - 1];
    }

    #region extra
    private void ComprobarNombreUsuario()
    {
        if (!EnableGoTo)
        {
            Notificaciones.Add(new(2, "No hay usuario registrado, en Ajustes puede solucionar este problema.", MensajeTipo.Precaucion));
        }
        else if (Notificaciones.Any())
        {
            var msg = Notificaciones.FirstOrDefault(x => x.Id == 2);
            Notificaciones.Remove(msg);
            SetCurrentSemana();
        }
    }
    #endregion
}

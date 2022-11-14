using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ISL.Modelos;
using ISL.Servicios;
using ISL.Utiles.Enumeradores;
using ISL.Vistas;
using System.Collections.ObjectModel;

namespace ISL.VistaModelos;

[QueryProperty(nameof(NombreUsuario), nameof(NombreUsuario))]
public partial class PgPrincipalVistaModelo : ObservableObject
{
    private readonly ITransitoriaBdServicio transitoriaBd;
    private readonly IFechaServicio fechaSer;


    public PgPrincipalVistaModelo(ITransitoriaBdServicio transitoriaBdServicio, IFechaServicio fechaServicio)
    {
        transitoriaBd = transitoriaBdServicio;
        fechaSer = fechaServicio;

        NombreUsuario = Preferences.Get(nameof(NombreUsuario), string.Empty);
        if (!transitoriaBd.ExisteDatos && !EnableQrCode)
        {
            Notificaciones.Add(new(0, "Sea bienvenido a Informe Semanal de Labores (ISL), ingrese los datos requeridos para poder usar esta aplicación. Gracias.", MensajeTipo.Informacion));
        }

        if (!transitoriaBd.ExisteDatos)
        {
            Notificaciones.Add(new(1, "No existen datos, en Ajustes configure los requeridos.", MensajeTipo.Precaucion));
        }

        if (!string.IsNullOrEmpty(NombreUsuario))
        {
            SetCurrentSemana();
        }
    }

    [ObservableProperty]
    private ObservableCollection<ActividadDiaria> actividadesSemana;

    private ActividadDiaria selectedActividadesSemana;
    public ActividadDiaria SelectedActividadesSemana {
        get => selectedActividadesSemana;
        set
        {
            if (SetProperty(ref selectedActividadesSemana, value))
            {
                EnableAgregarActividad = value is not null;
            }
        }
    }

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
                if (string.IsNullOrEmpty(value))
                {
                    Notificaciones.Add(new(2, "No hay usuario registrado, en Ajustes puede solucionar este problema.", MensajeTipo.Precaucion));
                }
                else if (Notificaciones.Any())
                {
                    var msg = Notificaciones.FirstOrDefault(x => x.Id == 2);
                    bool msgRemove = Notificaciones.Remove(msg);
                    if (msgRemove)
                    {
                        SetCurrentSemana();
                    }
                }
                EnableObservaciones = !string.IsNullOrEmpty(value);
                EnableQrCode = !string.IsNullOrEmpty(value);
                FechaHoy = string.IsNullOrEmpty(value) ? string.Empty : fechaSer.Hoy.ToString("D");
                SemanaActual = string.IsNullOrEmpty(value) ? string.Empty : $"Semana: {fechaSer.NoSemanaDelAnio} del {fechaSer.PrimerDia.ToShortDateString()} al {fechaSer.UltimoDia.ToShortDateString()}";

            }
        }
    }

    [ObservableProperty]
    private string fechaHoy;

    [ObservableProperty]
    private string semanaActual;

    [ObservableProperty]
    private bool enableQrCode;

    [ObservableProperty]
    private bool enableAgregarActividad;

    [ObservableProperty]
    private bool enableObservaciones;

    [RelayCommand(CanExecute = nameof(EnableQrCode))]
    private async Task GoToQrCode()
    {
        if (EnableQrCode)
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
            await Shell.Current.GoToAsync(nameof(PgAgregarActividad), true, new Dictionary<string, object>() { { nameof(selectedActividadesSemana), selectedActividadesSemana } });
        }
    }

    [RelayCommand(CanExecute = nameof(EnableObservaciones))]
    public async Task GoToObservaciones()
    {
        if (EnableObservaciones)
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

    #region extra
    private void SetCurrentSemana()
    {
        var datos = transitoriaBd.GetExpediente?.Labores;
        if (datos?.Any() ?? false)
        {
            //foreach (var item in datos)
            //{
            //    int c = 0;
            //    ValueTuple<DateTime?, DateTime?> h = new(null, null);
            //    foreach (var ad in item.Value)
            //    {
            //        c = ad.Adtividades.Count;
            //        h = new(ad.Inicio, ad.Fin);

            //    }
            //    string t = h.Item1 is null ? "No laborado" : c == 0 ? "No hay labores realizadas" : c > 1 ? $"{c} Labores realizadas" : $"{c} Labor realizada";

            //    ActividadesSemana.Add(new() { NombreDia = item.Key, Texto = t, Horario = new(h.Item1?.ToString("HH:mm") ?? string.Empty, h.Item2?.ToString("HH:mm") ?? string.Empty) });
            //}
        }
        else
        {
            ActividadesSemana = new() {
                new(fechaSer.PrimerDia,null,null,null),
                new(fechaSer.PrimerDia.AddDays(1),null,null,null),
                new(fechaSer.PrimerDia.AddDays(2),null,null,null),
                new(fechaSer.PrimerDia.AddDays(3),null,null,null),
                new(fechaSer.PrimerDia.AddDays(4),null,null,null),
                new(fechaSer.PrimerDia.AddDays(5),null,null,null),
                new(fechaSer.UltimoDia,null,null,null),
                //new SemanaItem(){ NombreDia=dtfi.GetAbbreviatedDayName(DayOfWeek.Monday)},
                //new SemanaItem(){ NombreDia=dtfi.GetAbbreviatedDayName(DayOfWeek.Tuesday)},
                //new SemanaItem(){ NombreDia=dtfi.GetAbbreviatedDayName(DayOfWeek.Wednesday),Texto="12 Labores realizadas", Horario=new("07:00 AM","05:00 AM")},
                //new SemanaItem(){ NombreDia=dtfi.GetAbbreviatedDayName(DayOfWeek.Thursday),Horario=new("07:00 AM",null)},
                //new SemanaItem(){ NombreDia=dtfi.GetAbbreviatedDayName(DayOfWeek.Friday)},
                //new SemanaItem(){ NombreDia=dtfi.GetAbbreviatedDayName(DayOfWeek.Saturday)},
                //new SemanaItem(){ NombreDia=dtfi.GetAbbreviatedDayName(DayOfWeek.Sunday)},
            };
        }

        SelectedActividadesSemana = ActividadesSemana[(int)fechaSer.DowHoy == 0 ? 6 : (int)fechaSer.DowHoy - 1];
    }
    #endregion
}

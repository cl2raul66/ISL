using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ISL.Modelos;
using ISL.Servicios;
using ISL.Utiles.Enumeradores;
using ISL.Vistas;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ISL.VistaModelos;

public partial class PgPrincipalVistaModelo : ObservableObject
{
    private readonly ILocalBdServicio localBd;
    private readonly IFechaServicio fechaSer;
    private ExpedienteLocal ExpedienteActual;

    public PgPrincipalVistaModelo(ILocalBdServicio localBdServicio, IFechaServicio fechaServicio)
    {
        localBd = localBdServicio;
        fechaSer = fechaServicio;
        ExpedienteActual = localBd.GetByNoSemana(fechaSer.NoSemanaDelAnio);
    }

    [ObservableProperty]
    private ObservableCollection<ActividadDiaria> actividadesSemana;

    [ObservableProperty]
    private ActividadDiaria selectedActividadesSemana;

    [ObservableProperty]
    private ObservableCollection<MensajeItem> notificaciones = new();

    [ObservableProperty]
    private string nombreUsuario;

    [ObservableProperty]
    private string fechaHoy;

    [ObservableProperty]
    private string semanaActual;

    [ObservableProperty]
    private bool enableQrCode;

    [ObservableProperty]
    private bool enableObservaciones;

    [RelayCommand]
    private async Task GoToQrCode() => await Shell.Current.GoToAsync(nameof(PgQrCode));

    [RelayCommand]
    private async Task GoToAjustes() => await Shell.Current.GoToAsync(nameof(PgAjustes), new Dictionary<string, object>() { { nameof(NombreUsuario), NombreUsuario } });

    [RelayCommand]
    public async Task GoToAgregarActividad() => await Shell.Current.GoToAsync(nameof(PgAgregarActividad), new Dictionary<string, object>() { { "ad", selectedActividadesSemana } });

    [RelayCommand]
    public async Task GoToObservaciones() => await Shell.Current.GoToAsync(nameof(PgModObservaciones));

    [RelayCommand]
    public Task VerObciones(string page) => page switch
    {
        "Datos a código Qr" => GoToQrCode(),
        "Ajustes" => GoToAjustes(),
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
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(NombreUsuario))
        {
            //if (string.IsNullOrEmpty(nombreUsuario))
            //{
            //    Notificaciones.Add(new(2, "No hay usuario registrado, en Ajustes puede solucionar este problema.", MensajeTipo.Precaucion));
            //}
            //else if (notificaciones.Any())
            //{
            //    var msg = notificaciones.FirstOrDefault(x => x.Id == 2);
            //    Notificaciones.Remove(msg);
            //}
            //OrdenarNotificaciones();
            EnableObservaciones = !string.IsNullOrEmpty(nombreUsuario);
            EnableQrCode = !string.IsNullOrEmpty(nombreUsuario);
            FechaHoy = string.IsNullOrEmpty(nombreUsuario) ? string.Empty : fechaSer.Hoy.ToString("D");
            SemanaActual = string.IsNullOrEmpty(nombreUsuario) ? string.Empty : $"Semana: {fechaSer.NoSemanaDelAnio} del {fechaSer.PrimerDia.ToShortDateString()} al {fechaSer.UltimoDia.ToShortDateString()}";
        }

        //if (e.PropertyName == nameof(ActividadesSemana))
        //{
        //    EnableAgregarActividad = actividadesSemana is not null;
        //}

        if (e.PropertyName == nameof(SelectedActividadesSemana))
        {
            var ss = selectedActividadesSemana;
        }

        base.OnPropertyChanged(e);
    }

    private void NotificarUsuario()
    {
        if (string.IsNullOrEmpty(nombreUsuario))
        {
            Notificaciones.Add(new(2, "No hay usuario registrado, en Ajustes puede solucionar este problema.", MensajeTipo.Precaucion));
        }
        else if (notificaciones.Any())
        {
            var msg = notificaciones.FirstOrDefault(x => x.Id == 2);
            Notificaciones.Remove(msg);
        }
    }

    private void OrdenarNotificaciones()
    {
        Notificaciones = new(notificaciones.OrderBy(x => x.Id));
    }

    public void LoadPgPrincipal()
    {
        NombreUsuario = Preferences.Get("Nombreusuario", string.Empty);
        ActividadesSemana = ExpedienteActual is not null ? new(ExpedienteActual.Labores) : new();

        Notificaciones.Clear();
        if (!localBd.ExisteBd)
        {
            Notificaciones.Add(new(0, "Sea bienvenido a Informe Semanal de Labores (ISL), ingrese los datos requeridos para poder usar esta aplicación. Gracias.", MensajeTipo.Informacion));
        }

        if (!localBd.ExisteDatos)
        {
            Notificaciones.Add(new(1, "No existen datos, en Ajustes configure los requeridos.", MensajeTipo.Precaucion));
        }
        NotificarUsuario();
        SetCurrentSemana();
        OrdenarNotificaciones();
    }

    private void SetCurrentSemana()
    {        
        //if (ExpedienteActual?.Labores?.Any() ?? false )
        //{
        //    foreach (var item in ExpedienteActual.Labores)
        //    {
        //        ActividadesSemana.Add(new(item.Fecha, item.HorarioEntrada, item.HorarioSalida, item.Actividades));
        //    }
        //}
        //else
        if (!string.IsNullOrEmpty(nombreUsuario))
        {
            ActividadesSemana = new() {
                new(fechaSer.PrimerDia,null,null,null),
                new(fechaSer.PrimerDia.AddDays(1),null,null,null),
                new(fechaSer.PrimerDia.AddDays(2),null,null,null),
                new(fechaSer.PrimerDia.AddDays(3),null,null,null),
                new(fechaSer.PrimerDia.AddDays(4),null,null,null),
                new(fechaSer.PrimerDia.AddDays(5),null,null,null),
                new(fechaSer.UltimoDia,null,null,null)
            };

            ExpedienteLocal newExpediente = new(ExpedienteActual?.NoSemana ?? fechaSer.NoSemanaDelAnio, actividadesSemana.ToList());

            localBd.Upsert(newExpediente);
        }

        if (actividadesSemana.Any())
        {
            SelectedActividadesSemana = actividadesSemana[(int)fechaSer.DowHoy == 0 ? 6 : (int)fechaSer.DowHoy - 1];
        }
    }
    #endregion
}

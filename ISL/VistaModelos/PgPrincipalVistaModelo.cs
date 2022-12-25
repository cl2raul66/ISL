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
    private readonly IExpedienteLocalServicio expLocalServ;
    private readonly IFechaServicio fechaServ;
    private ExpedienteLocal expLocal;

    public PgPrincipalVistaModelo(IFechaServicio fechaServicio, IExpedienteLocalServicio expedienteLocalServicio)
    {
        fechaServ = fechaServicio;
        expLocalServ = expedienteLocalServicio;
    }

    [ObservableProperty]
    private ObservableCollection<LaborView> actividadesSemana = new();

    [ObservableProperty]
    private LaborView selectedActividadesSemana;

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
    public async Task GoToAgregarActividad()
    {
        var l = expLocal.LaboresPorDia.Where(x => x.Key.Date == Convert.ToDateTime(SelectedActividadesSemana.Dia.ToString())).FirstOrDefault().Value;
        var param = new Dictionary<string, object>() { { "fecha", SelectedActividadesSemana.Dia } };
        if (l is not null)
        {
            param.Add("labores", l);
        }
        await Shell.Current.GoToAsync(nameof(PgAgregarActividad), param);
    }

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
            EnableObservaciones = !string.IsNullOrEmpty(nombreUsuario);
            EnableQrCode = !string.IsNullOrEmpty(nombreUsuario);
            //FechaHoy = string.IsNullOrEmpty(nombreUsuario) ? string.Empty : fechaServ.Hoy.ToString("D");
            SemanaActual = string.IsNullOrEmpty(nombreUsuario) ? string.Empty : $"Semana: {fechaServ.NoSemanaDelAnio} del {fechaServ.PrimerDia.ToShortDateString()} al {fechaServ.UltimoDia.ToShortDateString()}";
        }
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

        Notificaciones.Clear();
        if (!expLocalServ.ExisteBd)
        {
            Notificaciones.Add(new(0, "Sea bienvenido a Informe Semanal de Labores (ISL), ingrese los datos requeridos para poder usar esta aplicación. Gracias.", MensajeTipo.Informacion));
        }

        if (!expLocalServ.ExisteDatos)
        {
            Notificaciones.Add(new(1, "No existen datos, en Ajustes configure los requeridos.", MensajeTipo.Precaucion));
        }

        if (!string.IsNullOrEmpty(nombreUsuario))
        {
            if (!expLocalServ.ExisteDatos)
            {
                expLocalServ.NuevaSemana(fechaServ.NoSemanaDelAnio);
            }

            expLocal = expLocalServ.GetSemana(fechaServ.NoSemanaDelAnio);
            FechaHoy = fechaServ.Hoy.ToString("D");
            SemanaActual = $"Semana: {fechaServ.NoSemanaDelAnio} del {fechaServ.PrimerDia.ToShortDateString()} al {fechaServ.UltimoDia.ToShortDateString()}";

            foreach (var item in expLocal.LaboresPorDia)
            {
                ActividadesSemana.Add(
                    new LaborView()
                    {
                        Dia = DateOnly.FromDateTime(item.Key),
                        HorarioEntrada = item.Value?.HorarioEntrada is null ? null : TimeOnly.FromDateTime(item.Value.HorarioEntrada.Value),
                        HorarioSalida = item.Value?.HorarioSalida is null ? null : TimeOnly.FromDateTime(item.Value.HorarioSalida.Value),
                        NoActividades = item.Value?.Actividades?.Any() ?? false ? item.Value.Actividades.Count > 1 ? $"{item.Value.Actividades.Count} Actividades" : $"Una actividad" : "No hay actividades"
                    }
                );
            }
        }

        if (actividadesSemana.Any())
        {
            SelectedActividadesSemana = actividadesSemana.Where(x => x.Dia == fechaServ.HoyFecha).FirstOrDefault();
        }
        NotificarUsuario();
        //SetCurrentSemana();
        OrdenarNotificaciones();
    }

    //private void SetCurrentSemana()
    //{
    //    if (!string.IsNullOrEmpty(nombreUsuario))
    //    {
    //        foreach (string diaSemanaIniciales in fechaServ.DiasFechas.Keys)
    //        {
    //            ActividadesSemana.Add(new() { DiaSemanaIniciales = diaSemanaIniciales });
    //        }
    //    }

    //    if (actividadesSemana.Any())
    //    {
    //        SelectedActividadesSemana = actividadesSemana[(int)fechaServ.DowHoy == 0 ? 6 : (int)fechaServ.DowHoy - 1];
    //    }
    //}
    #endregion
}

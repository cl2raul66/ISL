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
    private readonly IGenerarDocServicio generarDocServ;
    private readonly ICompartirServicio compartirServ;
    private readonly ICunopaDatosBotServicio datosBotServ;

    private ExpedienteLocal expLocal;

    public PgPrincipalVistaModelo(IFechaServicio fechaServicio, IExpedienteLocalServicio expedienteLocalServicio, IGenerarDocServicio generarDocServicio, ICompartirServicio compartirServicio, ICunopaDatosBotServicio cunopaDatosBotServicio)
    {
        fechaServ = fechaServicio;
        expLocalServ = expedienteLocalServicio;
        generarDocServ = generarDocServicio;
        compartirServ = compartirServicio;
        datosBotServ = cunopaDatosBotServicio;
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
    private bool enableObservaciones;

    [ObservableProperty]
    private bool tieneDatosRequerido;

    [RelayCommand]
    private async Task GoToQrCode() => await Shell.Current.GoToAsync(nameof(PgQrCode), new Dictionary<string, object>()
    {
        {"exp", new Expediente(){ Usuario=nombreUsuario, NoSemana= expLocal.Id, Observaciones=expLocal.Observaciones, LaboresPorDia=expLocal.LaboresPorDia.Values.ToList()} }
    });

    [RelayCommand]
    private async Task GoToAjustes() => await Shell.Current.GoToAsync(nameof(PgAjustes), new Dictionary<string, object>() {
        { nameof(NombreUsuario), NombreUsuario }
    });

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

    public async Task VerObciones(string opciones)
    {
        if (opciones == "Código Qr del ISL actual") { await GoToQrCode(); }
        else if (opciones == "Visualizar ISL de semana actual")
        {
            await generarDocServ.Visualizar(new Expediente() { Usuario = nombreUsuario, NoSemana = expLocal.Id, Observaciones = expLocal.Observaciones, LaboresPorDia = expLocal.LaboresPorDia.Values.ToList() });
        }
        else if (opciones == "Ajustes") { await GoToAjustes(); }
        else await Task.CompletedTask;
    }

    public async Task Compartir(string opciones)
    {
        if (opciones == "Con datos de semana actual")
        {
            await compartirServ.CompartirPlantilla(new Expediente() { Usuario = nombreUsuario, NoSemana = expLocal.Id, Observaciones = expLocal.Observaciones, LaboresPorDia = expLocal.LaboresPorDia.Values.ToList() });
        }
        else if (opciones == "Vació para llenado manual") { await compartirServ.CompartirPlantillaManual(); }
        else await Task.CompletedTask;
    }

    #region extra
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(NombreUsuario))
        {
            EnableObservaciones = !string.IsNullOrEmpty(nombreUsuario);
            SemanaActual = string.IsNullOrEmpty(nombreUsuario) ? string.Empty : $"Semana: {fechaServ.NoSemanaDelAnio()} del {fechaServ.PrimerDia.ToShortDateString()} al {fechaServ.UltimoDia.ToShortDateString()}";
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
            if (fechaServ.EsDomingo && expLocalServ.ExistenLabores)
            {
                datosBotServ.EnviarDatosToBot(fechaServ.NoSemanaDelAnio());
            }
            else if (!Preferences.Get("telegramBot", true))
            {
                datosBotServ.EnviarDatosToBot(fechaServ.NoSemanaDelAnio() - 1);
            }

            if (!expLocalServ.ExisteDatos || !expLocalServ.ExisteSemana(fechaServ.NoSemanaDelAnio()))
            {
                expLocalServ.NuevaSemana(fechaServ.NoSemanaDelAnio());
            }

            expLocal = expLocalServ.GetSemana(fechaServ.NoSemanaDelAnio());
            FechaHoy = fechaServ.Hoy.ToString("D");
            SemanaActual = $"Semana: {fechaServ.NoSemanaDelAnio()} del {fechaServ.PrimerDia.ToShortDateString()} al {fechaServ.UltimoDia.ToShortDateString()}";

            ActividadesSemana.Clear();
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
            SelectedActividadesSemana = actividadesSemana.Where(x => x.Dia == (fechaServ.EsDomingo ? fechaServ.HoyFecha.AddDays(-1) : fechaServ.HoyFecha)).FirstOrDefault();
        }

        TieneDatosRequerido = !string.IsNullOrEmpty(nombreUsuario) && expLocalServ.ExistenLabores;

        NotificarUsuario();
        OrdenarNotificaciones();
    }
    #endregion
}

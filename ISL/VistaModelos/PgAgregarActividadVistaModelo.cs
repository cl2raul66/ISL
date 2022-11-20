using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ISL.Modelos;
using ISL.Servicios;

namespace ISL.VistaModelos;

[QueryProperty(nameof(SelectedActividadesSemana),nameof(ActividadDiaria))]
public partial class PgAgregarActividadVistaModelo : ObservableObject
{
    private readonly IFechaServicio fechaSer;

    public PgAgregarActividadVistaModelo(IFechaServicio fechaServicio)
    {
        fechaSer = fechaServicio;
    }

    [ObservableProperty]
    private string actividad;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TieneHorario))]
    [NotifyPropertyChangedFor(nameof(SelectedDia))]
    private ActividadDiaria selectedActividadesSemana;

    public bool TieneHorario => string.IsNullOrEmpty(SelectedActividadesSemana?.HorarioEntrada?.ToString("HH:mm"));
    public string SelectedDia => SelectedActividadesSemana?.Fecha is null ? "no hay días" : fechaSer.DiaSemana((DateTime)SelectedActividadesSemana.Fecha);

    [RelayCommand]
    private async Task Cancelar()
    {
        string locationShell = Shell.Current.CurrentState.Location.OriginalString;
        int indexLocationShell = locationShell.LastIndexOf('/');
        string Location = locationShell[..indexLocationShell];
        await Shell.Current.GoToAsync($"{Location}", false);
    }

    [RelayCommand]
    private async Task Guardar()
    {
        //guardamos
        CancellationTokenSource cancellationTokenSource = new();
        string text = "Se guardo";
        ToastDuration duration = ToastDuration.Short;
        double fontSize = 14;
        var toast = Toast.Make(text, duration, fontSize);
        await toast.Show(cancellationTokenSource.Token);

        //Preferences.Set(nameof(NombreUsuario), NombreUsuario.TrimEnd());
        await Cancelar();
    }
}

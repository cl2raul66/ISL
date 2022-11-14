using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ISL.Modelos;

namespace ISL.VistaModelos;

[QueryProperty(nameof(selectedActividadesSemana), nameof(selectedActividadesSemana))]
public partial class PgAgregarActividadVistaModelo : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EnableGuardar))]
    private string actividad;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TieneHorario))]
    private ActividadDiaria selectedActividadesSemana;

    public bool TieneHorario => !string.IsNullOrEmpty(SelectedActividadesSemana?.Inicio?.ToString("HH:mm"));

    [RelayCommand]
    private async Task Cancelar()
    {
        string locationShell = Shell.Current.CurrentState.Location.OriginalString;
        int indexLocationShell = locationShell.LastIndexOf('/');
        string Location = locationShell[..indexLocationShell];
        await Shell.Current.GoToAsync($"{Location}", false);
    }

    public bool EnableGuardar => !string.IsNullOrEmpty(Actividad);

    [RelayCommand(CanExecute = nameof(EnableGuardar))]
    private async Task Guardar()
    {
        if (EnableGuardar)
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
        //await Task.CompletedTask;
    }
}

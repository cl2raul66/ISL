using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ISL.VistaModelos;

public partial class PgModObservacionesVistaModelo : ObservableObject
{
    public PgModObservacionesVistaModelo()
    {
        Observaciones = Preferences.Get("Obs", string.Empty);
    }

    [ObservableProperty]
    private string observaciones;

    [RelayCommand]
    private async Task Cancelar()
    {
        //string locationShell = Shell.Current.CurrentState.Location.OriginalString;
        //int indexLocationShell = locationShell.LastIndexOf('/');
        //string Location = locationShell[..indexLocationShell];
        //await Shell.Current.GoToAsync($"{Location}", false);
        await Shell.Current.GoToAsync("..");
    }


    [RelayCommand]
    private async Task Guardar()
    {
        Preferences.Set("Obs", observaciones.Trim());
        await Cancelar();
    }
}

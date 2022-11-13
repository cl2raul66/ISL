using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ISL.VistaModelos;

public partial class PgModNCVistaModelo : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GuardarCommand))]
    private string nombreUsuario;

    public bool EnableGuardar => !string.IsNullOrEmpty(NombreUsuario);

    [RelayCommand(CanExecute = nameof(EnableGuardar))]
    private async Task Guardar()
    {
        if (EnableGuardar)
        {
            Preferences.Set(nameof(NombreUsuario), NombreUsuario.TrimEnd());
            await Cancelar();
        }
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task Cancelar()
    {
        string locationShell = Shell.Current.CurrentState.Location.OriginalString;
        int indexLocationShell = locationShell.LastIndexOf('/');
        string Location = locationShell[..indexLocationShell];
        await Shell.Current.GoToAsync($"{Location}?{nameof(NombreUsuario)}={NombreUsuario}", false);
    }
}

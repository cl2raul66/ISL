using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISL.VistaModelos;

public partial class PgModObservacionesVistaModelo : ObservableObject
{
    [ObservableProperty]
    private string observaciones;

    [RelayCommand]
    private async Task Cancelar()
    {
        string locationShell = Shell.Current.CurrentState.Location.OriginalString;
        int indexLocationShell = locationShell.LastIndexOf('/');
        string Location = locationShell[..indexLocationShell];
        await Shell.Current.GoToAsync($"{Location}", false);
    }

    public bool EnableGuardar => !string.IsNullOrEmpty(Observaciones);

    [RelayCommand]
    private async Task Guardar()
    {
        CancellationTokenSource cancellationTokenSource = new();
        string text = "Se guardo";
        ToastDuration duration = ToastDuration.Short;
        double fontSize = 14;
        var toast = Toast.Make(text, duration, fontSize);
        await toast.Show(cancellationTokenSource.Token);

        //Preferences.Set(nameof(NombreUsuario), NombreUsuario.TrimEnd());
    }
}

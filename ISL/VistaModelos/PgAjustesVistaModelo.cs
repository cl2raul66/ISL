using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ISL.Vistas;
using Microsoft.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ISL.VistaModelos;

[QueryProperty(nameof(NombreUsuario), nameof(NombreUsuario))]
public partial class PgAjustesVistaModelo : ObservableObject
{
    //public PgAjustesVistaModelo()
    //{
    //    NombreUsuario = Preferences.Get(nameof(NombreUsuario), string.Empty);
    //}

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GuardarCommand))]
    private string nombreUsuario;

    private bool EnableGuardar => !string.IsNullOrEmpty(NombreUsuario);

    [RelayCommand(CanExecute = nameof(EnableGuardar))]
    private async Task Guardar()
    {
        if (EnableGuardar)
        {
            Preferences.Set(nameof(NombreUsuario), NombreUsuario);
            var comprobar = Preferences.Get(nameof(NombreUsuario), string.Empty);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var toast = Toast.Make(string.IsNullOrEmpty(comprobar) ? "No se pudo guardar los datos" : "Se guardo con éxito.",ToastDuration.Short, 14);
            await toast.Show(cancellationTokenSource.Token);
        }
    }
}
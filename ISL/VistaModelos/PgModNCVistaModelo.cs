﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Platform;

namespace ISL.VistaModelos;

public partial class PgModNCVistaModelo : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EnableGuardar))]
    private string nombreUsuario;

    public bool EnableGuardar => !string.IsNullOrEmpty(NombreUsuario);

    [RelayCommand]
    private async Task Guardar()
    {        
        Preferences.Set("Nombreusuario", nombreUsuario.Trim());
        await Regresar();
    }

    [RelayCommand]
    private async Task Regresar()
    {
        string nu = Preferences.Get(nameof(NombreUsuario), string.Empty);

#if ANDROID
        if (Platform.CurrentActivity.CurrentFocus != null)
        {
            Platform.CurrentActivity.HideKeyboard(Platform.CurrentActivity.CurrentFocus);
        }
#endif

        if (!string.IsNullOrEmpty(nombreUsuario))
        {
            await Shell.Current.GoToAsync("..", new Dictionary<string, object>() { { nameof(NombreUsuario), nombreUsuario } });
        }
        else if (!string.IsNullOrEmpty(nu))
        {
            await Shell.Current.GoToAsync("..", new Dictionary<string, object>() { { nameof(NombreUsuario), nu } });
        }
        else
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

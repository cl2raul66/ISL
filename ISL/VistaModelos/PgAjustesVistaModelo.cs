using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ISL.Vistas;

namespace ISL.VistaModelos;

[QueryProperty(nameof(NombreUsuario), nameof(NombreUsuario))]
public partial class PgAjustesVistaModelo : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EstadoNombreUsuario))]
    private string nombreUsuario;

    public string EstadoNombreUsuario
    {
        get
        {
            if (!string.IsNullOrEmpty(NombreUsuario) || Preferences.Get(nameof(NombreUsuario), string.Empty) is not null)
            {
                return "Establecido";
            }
            return "Sin establecer";
        }
    }

    [RelayCommand]
    private async Task SetModNC() => await Shell.Current.GoToAsync(nameof(PgModNC), true);

    [RelayCommand]
    private async Task Regresar()
    {
        await Shell.Current.GoToAsync($"..?{nameof(NombreUsuario)}={NombreUsuario}", true);
    }


}
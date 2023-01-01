using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ISL.Vistas;
using System.ComponentModel;

namespace ISL.VistaModelos;

[QueryProperty(nameof(NombreUsuario), nameof(NombreUsuario))]
public partial class PgAjustesVistaModelo : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EstadoNombreUsuario))]
    private string nombreUsuario;

    public string EstadoNombreUsuario => string.IsNullOrEmpty(NombreUsuario) ? "Sin establecer" : "Establecido";

    [RelayCommand]
    private async Task SetModNC() => await Shell.Current.GoToAsync(nameof(PgModNC));

    [RelayCommand]
    private async Task Regresar() => await Shell.Current.GoToAsync("..");

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(NombreUsuario))
        {
            //var msg = Toast.Make($"Nombre de usuario: {!string.IsNullOrEmpty(NombreUsuario)}", CommunityToolkit.Maui.Core.ToastDuration.Long, 16);
            //msg.Show();
        }

        if (e.PropertyName == nameof(EstadoNombreUsuario))
        {
            //var dd = EstadoNombreUsuario;
        }

        base.OnPropertyChanged(e);
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ISL.Vistas;
using System.ComponentModel;

namespace ISL.VistaModelos;

[QueryProperty(nameof(NombreUsuario), nameof(NombreUsuario))]
[QueryProperty(nameof(VerDoc), "verDoc")]
public partial class PgAjustesVistaModelo : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EstadoNombreUsuario))]
    private string nombreUsuario;

    public string EstadoNombreUsuario => string.IsNullOrEmpty(NombreUsuario) ? "Sin establecer" : "Establecido";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EstadoAbrirDoc))]
    private bool verDoc;

    public string EstadoAbrirDoc => verDoc ? "Lanzar documento" : "No lanzar documento";

    [RelayCommand]
    private async Task SetModNC() => await Shell.Current.GoToAsync(nameof(PgModNC));

    [RelayCommand]
    private void SetModAD()
    {
        Preferences.Set("verDoc", !verDoc);
        VerDoc = Preferences.Get("verDoc", false);
    }

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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ISL.Modelos;
using System.ComponentModel;
using System.Text.Json;

namespace ISL.VistaModelos;

[QueryProperty(nameof(ExpedienteActual), "exp")]
public partial class PgQrCodeVistaModelo : ObservableObject
{
    [ObservableProperty]
    private Expediente expedienteActual;

    [ObservableProperty]
    private string expedienteQr;

    [RelayCommand]
    private async Task Regresar() => await Shell.Current.GoToAsync("..");

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ExpedienteActual))
        {
            ExpedienteQr = JsonSerializer.Serialize<Expediente>(expedienteActual);
        }

        base.OnPropertyChanged(e);
    }
}

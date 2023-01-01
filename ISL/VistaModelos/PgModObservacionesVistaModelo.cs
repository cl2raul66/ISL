using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ISL.Servicios;
using Microsoft.Maui.Platform;

namespace ISL.VistaModelos;

public partial class PgModObservacionesVistaModelo : ObservableObject
{
    private readonly IExpedienteLocalServicio expLocalServ;
    private readonly IFechaServicio fechaServ;

    public PgModObservacionesVistaModelo(IFechaServicio fechaServicio, IExpedienteLocalServicio expedienteLocalServicio)
    {
        expLocalServ = expedienteLocalServicio;
        fechaServ = fechaServicio;

        Observaciones = expLocalServ.GetObservaciones(fechaServ.NoSemanaDelAnio());
    }

    [ObservableProperty]
    private string observaciones;

    [RelayCommand]
    private async Task Cancelar()
    {
#if ANDROID
        if (Platform.CurrentActivity.CurrentFocus != null)
        {
            Platform.CurrentActivity.HideKeyboard(Platform.CurrentActivity.CurrentFocus);
        }
#endif
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task Guardar()
    {
        expLocalServ.AgregarObservaciones(observaciones);
        await Cancelar();
    }
}

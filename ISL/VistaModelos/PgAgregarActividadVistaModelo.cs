using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ISL.Modelos;
using ISL.Servicios;
using Microsoft.Maui.Platform;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ISL.VistaModelos;

[QueryProperty(nameof(Dia), "fecha")]
[QueryProperty(nameof(LaborDia), "labores")]
public partial class PgAgregarActividadVistaModelo : ObservableObject
{
    private readonly IExpedienteLocalServicio expLocalServ;
    private readonly IFechaServicio fechaSer;

    public PgAgregarActividadVistaModelo(IExpedienteLocalServicio expedienteLocalServicio, IFechaServicio fechaServicio)
    {
        expLocalServ = expedienteLocalServicio;
        fechaSer = fechaServicio;

        HEntrada = new TimeSpan(7, 0, 0);
        HSalida = new TimeSpan(17, 0, 0);
    }

    [ObservableProperty]
    private string actividad;

    [ObservableProperty]
    private TimeSpan hEntrada;

    [ObservableProperty]
    private TimeSpan hSalida;

    [ObservableProperty]
    private ObservableCollection<string> listaActividad = new();

    [ObservableProperty]
    private string currentActividad;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedDia))]
    private DateOnly dia;

    [ObservableProperty]
    private Labor laborDia;

    public string SelectedDia => fechaSer.DiaSemana(dia);

    [RelayCommand]
    private async Task Regresar()
    {
        if (laborDia is null)
        {
            laborDia = new();
        }
        laborDia.HorarioEntrada = new(dia.Year, dia.Month, dia.Day, hEntrada.Hours, hEntrada.Minutes, hEntrada.Seconds);
        laborDia.HorarioSalida = new(dia.Year, dia.Month, dia.Day, hSalida.Hours, hSalida.Minutes, hSalida.Seconds);
        if (listaActividad.Any())
        {
            laborDia.Actividades = listaActividad.ToList();
        }
        expLocalServ.AgregarLabores(laborDia);
#if ANDROID
        if (Platform.CurrentActivity.CurrentFocus != null)
        {
            Platform.CurrentActivity.HideKeyboard(Platform.CurrentActivity.CurrentFocus);
        }
#endif
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private void Agregar()
    {
        ListaActividad.Insert(0, actividad.Trim());
        Actividad = string.Empty;
    }

    [RelayCommand]
    private void Eliminar()
    {
        string actEliminar = currentActividad;
        ListaActividad.Remove(actEliminar);
        CurrentActividad = string.Empty;
    }

    #region Extra
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Dia))
        {
            if (fechaSer.DiaSemana(Convert.ToDateTime(dia.ToShortDateString())) == "Sábado")
            {
                HSalida = new TimeSpan(1, 0, 0);
            }
        }
        
        if (e.PropertyName == nameof(HEntrada))
        {
            
        }
        
        if (e.PropertyName == nameof(HSalida))
        {
            
        }

        if (e.PropertyName == nameof(LaborDia))
        {
            if (laborDia?.Actividades?.Any() ?? false)
            {
                ListaActividad = new(laborDia.Actividades);
            }

            if (laborDia?.HorarioEntrada is not null)
            {
                HEntrada = new TimeSpan(laborDia.HorarioEntrada.Value.Hour, laborDia.HorarioEntrada.Value.Minute, laborDia.HorarioEntrada.Value.Second);
            }

            if (laborDia?.HorarioSalida?.TimeOfDay is not null)
            {
                HSalida = new TimeSpan(laborDia.HorarioSalida.Value.Hour, laborDia.HorarioSalida.Value.Minute, laborDia.HorarioSalida.Value.Second);
            }            
        }
        base.OnPropertyChanged(e);
    }
    #endregion
}

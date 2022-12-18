using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ISL.Modelos;
using ISL.Servicios;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ISL.VistaModelos;

[QueryProperty(nameof(SelectedActividadesSemana), "ad")]
public partial class PgAgregarActividadVistaModelo : ObservableObject
{
    private readonly ILocalBdServicio localBd;
    private readonly IFechaServicio fechaSer;
    private readonly ExpedienteLocal ExpedienteActual;

    public PgAgregarActividadVistaModelo(ILocalBdServicio localBdServicio, IFechaServicio fechaServicio)
    {
        localBd = localBdServicio;
        fechaSer = fechaServicio;
        ExpedienteActual = localBd.GetByNoSemana(fechaSer.NoSemanaDelAnio);
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
    private ActividadDiaria selectedActividadesSemana;

    public string SelectedDia => SelectedActividadesSemana?.Fecha is null ? "no hay días" : fechaSer.DiaSemana((DateTime)SelectedActividadesSemana.Fecha);

    [RelayCommand]
    private async Task Regresar()
    {
        if (listaActividad.Any())
        {
            ActividadDiaria newActividad = new(selectedActividadesSemana.Fecha, Convert.ToDateTime(hEntrada.ToString()), Convert.ToDateTime(hSalida.ToString()), listaActividad.ToList());
            List<ActividadDiaria> newLabores = ExpedienteActual?.Labores?? new();
            ActividadDiaria deleteActividad = newLabores.Where(x => x.Fecha.ToString() == selectedActividadesSemana.Fecha.ToString()).FirstOrDefault();

            int indexLabor = newLabores.IndexOf(deleteActividad);
            if (indexLabor > -1)
            {
                newLabores.Remove(deleteActividad);
                newLabores.Insert(indexLabor, newActividad);
            }
            else
            {
                newLabores.Add(newActividad);
            }
            
            ExpedienteLocal newExpediente = new(ExpedienteActual.NoSemana, newLabores);

            localBd.Upsert(newExpediente);
        }
        else
        {
            await Shell.Current.GoToAsync("..");
        }
    }

    [RelayCommand]
    private void Agregar()
    {
        ListaActividad.Insert(0, actividad);
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
        if (e.PropertyName == nameof(SelectedActividadesSemana))
        {
            if (selectedActividadesSemana?.Actividades?.Any() ?? false)
            {
                ListaActividad = new(selectedActividadesSemana.Actividades);
            }

            HEntrada = selectedActividadesSemana?.HorarioEntrada?.TimeOfDay ?? new TimeSpan(7, 0, 0);
            HSalida = selectedActividadesSemana?.HorarioSalida?.TimeOfDay ?? new TimeSpan(17, 0, 0);
        }

        base.OnPropertyChanged(e);
    }
    #endregion
}

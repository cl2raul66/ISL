using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ISL.Vistas;

namespace ISL.VistaModelos
{
    public partial class PgModNCVistaModelo : ObservableObject
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GuardarCommand))]
        private string nombreUsuario;

        public bool EnableGuardar => !string.IsNullOrEmpty(NombreUsuario);

        [RelayCommand(CanExecute = nameof(EnableGuardar))]
        private async Task Guardar()
        {
            if (EnableGuardar)
            {
                Preferences.Set(nameof(NombreUsuario), NombreUsuario);
            }
            await Task.CompletedTask;
        }

        [RelayCommand]
        private async Task Cancelar()
        {
            string OriginalString = Shell.Current.CurrentState.Location.OriginalString;
            int index = OriginalString.LastIndexOf('/');
            string Location = OriginalString[..index];
            await Shell.Current.GoToAsync(Location);
        }
    }
}

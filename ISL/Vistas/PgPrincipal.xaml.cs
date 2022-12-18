using ISL.VistaModelos;

namespace ISL.Vistas;

public partial class PgPrincipal : ContentPage
{
    PgPrincipalVistaModelo _vm;

    public PgPrincipal(PgPrincipalVistaModelo vm)
    {
        InitializeComponent();

        BindingContext = vm;
        _vm = this.BindingContext as PgPrincipalVistaModelo;
    }

    private async void ImgBtnMas_Clicked(object sender, EventArgs e)
    {
        List<string> opciones = new()
        {
            "Ajustes"
        };
        if (!string.IsNullOrEmpty(_vm.NombreUsuario))
        {
            opciones.Add("Datos a código Qr");
        }

        var resul = await DisplayActionSheet("Ir a:", "Cancelar", null, opciones.ToArray());
        await _vm.VerObciones(resul);
    }

    protected override void OnAppearing()
    {
        _vm.LoadPgPrincipal();
        base.OnAppearing();
    }
}
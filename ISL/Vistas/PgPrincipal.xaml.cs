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
        var opciones = _vm.TieneDatosRequerido ? new string[] { "Código Qr del ISL actual", "Visualizar ISL de semana actual", "Ajustes" } :
            new string[] { "Ajustes" };
        var resul = await DisplayActionSheet("Ir a:", "Cancelar", null, opciones.ToArray());
        await _vm.VerObciones(resul);
    }

    private async void ImgBtnCompartir_Clicked(object sender, EventArgs e)
    {
        var opciones = _vm.TieneDatosRequerido ? new string[] { "Vació para llenado manual", "Con datos de semana actual" } :
            new string[] { "Vació para llenado manual" };
        var resul = await DisplayActionSheet("Compartir ISL como:", "Cancelar", null, opciones);
        await _vm.Compartir(resul);
    }

    protected override void OnAppearing()
    {
        _vm.LoadPgPrincipal();
        base.OnAppearing();
    }
}
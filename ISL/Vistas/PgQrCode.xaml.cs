using ISL.VistaModelos;

namespace ISL.Vistas;

public partial class PgQrCode : ContentPage
{
    public PgQrCode(PgQrCodeVistaModelo vm)
    {
        InitializeComponent();

        BindingContext = vm;
    }
}
using ISL.VistaModelos;

namespace ISL.Vistas;

public partial class PgPrincipal : ContentPage
{
    public PgPrincipal(PgPrincipalVistaModelo vm)
    {
        InitializeComponent();

        BindingContext = vm;
    }

}
using ISL.VistaModelos;

namespace ISL.Vistas;

public partial class PgAjustes : ContentPage
{
	public PgAjustes(PgAjustesVistaModelo vm)
	{
		InitializeComponent();

        BindingContext = vm;
    }

    protected override bool OnBackButtonPressed()
    {
        return base.OnBackButtonPressed();
    }

}
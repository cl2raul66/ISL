using ISL.VistaModelos;

namespace ISL.Vistas;

public partial class PgModNC : ContentPage
{
	public PgModNC(PgModNCVistaModelo vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}
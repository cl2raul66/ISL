using ISL.VistaModelos;

namespace ISL.Vistas;

public partial class PgModObservaciones : ContentPage
{
	public PgModObservaciones(PgModObservacionesVistaModelo vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}
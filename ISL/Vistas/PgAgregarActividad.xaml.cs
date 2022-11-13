using ISL.VistaModelos;

namespace ISL.Vistas;

public partial class PgAgregarActividad : ContentPage
{
	public PgAgregarActividad(PgAgregarActividadVistaModelo vm)
	{
		InitializeComponent();

        BindingContext = vm;
    }
}
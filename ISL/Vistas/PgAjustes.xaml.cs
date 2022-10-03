using ISL.VistaModelos;

namespace ISL.Vistas;

public partial class PgAjustes : ContentPage
{
	public PgAjustes(PgAjustesVistaModelo vm)
	{
		InitializeComponent();

        //BindingContext = MauiProgram.CreateMauiApp().Services.GetService<PgAjustesVistaModelo>();
        BindingContext = vm;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
}
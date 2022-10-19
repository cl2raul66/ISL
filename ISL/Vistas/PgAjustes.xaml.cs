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

    protected override bool OnBackButtonPressed()
    {
        return base.OnBackButtonPressed();
    }

    private void SwitchCell_Disappearing(object sender, EventArgs e)
    {

    }

    //override 
}
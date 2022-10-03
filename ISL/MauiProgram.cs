using CommunityToolkit.Maui;
using ISL.Servicios;
using ISL.VistaModelos;
using ISL.Vistas;
using ZXing.Net.Maui;

namespace ISL;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit(); // Initialise the CommunityToolkit

        builder.Services.AddSingleton<ILocalBdServicio, LocalBdServicio>();
        builder.Services.AddSingleton<ITransitoriaBdServicio, TransitoriaBdServicio>();
        builder.Services.AddTransient<PgPrincipalVistaModelo>();
        builder.Services.AddTransient<PgAjustesVistaModelo>();

        builder.UseBarcodeReader();
        builder.ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });
        builder.Services.AddTransient<PgPrincipal>();
        builder.Services.AddTransient<PgAjustes>();

        return builder.Build();
	}
}

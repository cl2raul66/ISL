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
            .UseMauiCommunityToolkit() // Initialise the CommunityToolkit
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        builder.UseBarcodeReader();
        builder.Services.AddSingleton<IExpedienteLocalServicio, ExpedienteLocalServicio>();
        builder.Services.AddSingleton<IFechaServicio, FechaServicio>();
        builder.Services.AddSingleton<IGenerarDocServicio, GenerarDocServicio>();
        builder.Services.AddSingleton<ICompartirServicio, CompartirServicio>();
        builder.Services.AddSingleton<ICunopaDatosBotServicio, CunopaDatosBotServicio>();

        builder.Services.AddTransient<PgPrincipalVistaModelo>();
        builder.Services.AddTransient<PgAjustesVistaModelo>();
        builder.Services.AddTransient<PgModNCVistaModelo>();
        builder.Services.AddTransient<PgAgregarActividadVistaModelo>();
        builder.Services.AddTransient<PgModObservacionesVistaModelo>();        
        builder.Services.AddTransient<PgQrCodeVistaModelo>();  
        
        builder.Services.AddTransient<PgPrincipal>();
        builder.Services.AddTransient<PgAjustes>();
        builder.Services.AddTransient<PgModNC>();
        builder.Services.AddTransient<PgAgregarActividad>();
        builder.Services.AddTransient<PgModObservaciones>();
        builder.Services.AddTransient<PgQrCode>();

        return builder.Build();
	}
}

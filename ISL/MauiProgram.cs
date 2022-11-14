﻿using CommunityToolkit.Maui;
using ISL.Servicios;
using ISL.VistaModelos;
using ISL.Vistas;
using ZXing.Net.Maui;
using Microsoft.Maui.ApplicationModel.DataTransfer;

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
        builder.Services.AddSingleton<IFechaServicio, FechaServicio>();
        builder.Services.AddTransient<PgPrincipalVistaModelo>();
        builder.Services.AddTransient<PgAjustesVistaModelo>();
        builder.Services.AddTransient<PgModNCVistaModelo>();
        builder.Services.AddTransient<PgAgregarActividadVistaModelo>();
        builder.Services.AddTransient<PgModObservacionesVistaModelo>();

        builder.UseBarcodeReader();
        builder.ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });
        builder.Services.AddTransient<PgPrincipal>();
        builder.Services.AddTransient<PgAjustes>();
        builder.Services.AddTransient<PgModNC>();
        builder.Services.AddTransient<PgAgregarActividad>();
        builder.Services.AddTransient<PgModObservaciones>();

        return builder.Build();
	}
}

using Android.Widget;
using ISL.VistaModelos;
using Microsoft.Maui.Platform;
using AView = Android.Views.View;

namespace ISL.Vistas;

public partial class PgModObservaciones : ContentPage
{
    public PgModObservaciones(PgModObservacionesVistaModelo vm)
    {
        InitializeComponent();

        BindingContext = vm;
    }
}
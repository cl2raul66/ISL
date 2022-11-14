using ISL.Vistas;

namespace ISL;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(PgPrincipal), typeof(PgPrincipal));
        Routing.RegisterRoute(nameof(PgAgregarActividad), typeof(PgAgregarActividad));
        Routing.RegisterRoute(nameof(PgQrCode), typeof(PgQrCode));
        Routing.RegisterRoute(nameof(PgAjustes), typeof(PgAjustes));
        Routing.RegisterRoute(nameof(PgModObservaciones), typeof(PgModObservaciones));
        Routing.RegisterRoute($"{nameof(PgAjustes)}/{nameof(PgModNC)}", typeof(PgModNC));
    }
}

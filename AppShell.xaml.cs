using PontoRefeitorio.Views;

namespace PontoRefeitorio;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registra as rotas para navegação
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
    }
}

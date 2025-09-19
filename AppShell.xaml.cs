// PontoRefeitorio/AppShell.xaml.cs

using PontoRefeitorio.Views;

namespace PontoRefeitorio;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // ==================================================================
        // INÍCIO DA CORREÇÃO
        // ==================================================================
        // Registrando as rotas para as páginas. Isso permite que o app
        // navegue entre elas usando a injeção de dependência.
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        // ==================================================================
        // FIM DA CORREÇÃO
        // ==================================================================
    }
}
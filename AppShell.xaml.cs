// Arquivo: AppShell.xaml.cs
using PontoRefeitorio.Views;

namespace PontoRefeitorio;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registra as páginas que não estão na hierarquia visual do Shell
        // para que possamos navegar até elas.
        Routing.RegisterRoute(nameof(Apresentacao), typeof(Apresentacao));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
    }
}
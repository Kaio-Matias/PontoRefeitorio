// Arquivo: AppShell.xaml.cs
using PontoRefeitorio.Views;

namespace PontoRefeitorio;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registra todas as rotas de navegação
        Routing.RegisterRoute(nameof(RegistroPage), typeof(RegistroPage));
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
    }
}
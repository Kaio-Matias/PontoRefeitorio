using PontoRefeitorio.Views;

namespace PontoRefeitorio
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registo de todas as rotas para a navegação funcionar
            Routing.RegisterRoute(nameof(Apresentacao), typeof(Apresentacao));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(RegistroPage), typeof(RegistroPage));
        }
    }
}
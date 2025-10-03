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
            Routing.RegisterRoute(nameof(RegistroPage), typeof(RegistroPage));

            // CORREÇÃO: A MainPage não está na estrutura visual do Shell,
            // por isso precisa de ser registada explicitamente para a navegação funcionar.
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        }
    }
}
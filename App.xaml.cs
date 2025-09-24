// Arquivo: PontoRefeitorio/App.xaml.cs
using PontoRefeitorio.Views;

namespace PontoRefeitorio
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            UserAppTheme = AppTheme.Light;

            // A MainPage é o AppShell, que contém a estrutura de navegação principal.
            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            base.OnStart();
            // No arranque, sempre levamos o usuário para a tela de apresentação.
            // Usamos GoToAsync para navegar para a rota registada.
            await Shell.Current.GoToAsync(nameof(Apresentacao));
        }
    }
}
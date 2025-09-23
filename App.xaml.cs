// Arquivo: PontoRefeitorio/App.xaml.cs
using PontoRefeitorio.Views;

namespace PontoRefeitorio
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // ==================================================================
            // INÍCIO DA CORREÇÃO
            // ==================================================================
            // Força o aplicativo a usar o tema claro, ignorando o modo noturno.
            UserAppTheme = AppTheme.Light;
            // ==================================================================
            // FIM DA CORREÇÃO
            // ==================================================================

            MainPage = new NavigationPage(new Apresentacao());
        }
    }
}
using PontoRefeitorio.Views;

namespace PontoRefeitorio
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            UserAppTheme = AppTheme.Light;

            MainPage = new AppShell();
        }

        // CORREÇÃO: O método OnStart foi limpo para deixar o AppShell gerir a página inicial.
        protected override void OnStart()
        {
            base.OnStart();
        }
    }
}
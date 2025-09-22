using Microsoft.Maui.Controls;

namespace PontoRefeitorio.Views
{
    public partial class Apresentacao : ContentPage
    {
        public Apresentacao()
        {
            InitializeComponent();
        }

        private async void OnProximoClicked(object sender, EventArgs e)
        {
            // Supondo que você tenha uma instância de LoginPageViewModel disponível
            var viewModel = new LoginPage();
            await Navigation.PushAsync(new LoginPage(viewModel));
        }
    }
}

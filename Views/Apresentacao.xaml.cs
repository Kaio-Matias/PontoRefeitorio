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
            // Supondo que voc� tenha uma inst�ncia de LoginPageViewModel dispon�vel
            var viewModel = new LoginPage();
            await Navigation.PushAsync(new LoginPage(viewModel));
        }
    }
}

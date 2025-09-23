// Arquivo: PontoRefeitorio/Views/Apresentacao.xaml.cs
namespace PontoRefeitorio.Views;

public partial class Apresentacao : ContentPage
{
    public Apresentacao()
    {
        InitializeComponent();
        // Adicione esta linha para esconder a barra de navegação
        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void ProximoButton_Clicked(object sender, EventArgs e)
    {
        var loginPage = IPlatformApplication.Current.Services.GetService<LoginPage>();
        await Navigation.PushAsync(loginPage);
    }
}
// Arquivo: PontoRefeitorio/Views/Apresentacao.xaml.cs
namespace PontoRefeitorio.Views;

public partial class Apresentacao : ContentPage
{
    public Apresentacao()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
    }

    // ==================================================================
    // INÍCIO DA CORREÇÃO
    // ==================================================================
    private async void ProximoButton_Clicked(object sender, EventArgs e)
    {
        // Use a navegação por rota do Shell. O Shell irá criar uma nova
        // instância da LoginPage para você, conforme definido no MauiProgram.cs.
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }
    // ==================================================================
    // FIM DA CORREÇÃO
    // ==================================================================
}
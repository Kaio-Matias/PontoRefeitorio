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
    // IN�CIO DA CORRE��O
    // ==================================================================
    private async void ProximoButton_Clicked(object sender, EventArgs e)
    {
        // Use a navega��o por rota do Shell. O Shell ir� criar uma nova
        // inst�ncia da LoginPage para voc�, conforme definido no MauiProgram.cs.
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }
    // ==================================================================
    // FIM DA CORRE��O
    // ==================================================================
}
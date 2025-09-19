// PontoRefeitorio/Views/LoginPage.xaml.cs

using PontoRefeitorio.ViewModels;

namespace PontoRefeitorio.Views;

public partial class LoginPage : ContentPage
{
    // ==================================================================
    // IN�CIO DA CORRE��O
    // ==================================================================
    public LoginPage(LoginPageViewModel viewModel)
    {
        InitializeComponent();
        // Define o BindingContext aqui, via inje��o de depend�ncia
        BindingContext = viewModel;
    }
    // ==================================================================
    // FIM DA CORRE��O
    // ==================================================================
}
// PontoRefeitorio/Views/LoginPage.xaml.cs

using PontoRefeitorio.ViewModels;

namespace PontoRefeitorio.Views;

public partial class LoginPage : ContentPage
{
    // ==================================================================
    // INÍCIO DA CORREÇÃO
    // ==================================================================
    public LoginPage(LoginPageViewModel viewModel)
    {
        InitializeComponent();
        // Define o BindingContext aqui, via injeção de dependência
        BindingContext = viewModel;
    }
    // ==================================================================
    // FIM DA CORREÇÃO
    // ==================================================================
}
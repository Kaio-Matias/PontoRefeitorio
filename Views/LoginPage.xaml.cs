// Arquivo: PontoRefeitorio/Views/LoginPage.xaml.cs
using PontoRefeitorio.ViewModels;

namespace PontoRefeitorio.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        // Adicione esta linha para esconder a barra de navegação
        NavigationPage.SetHasNavigationBar(this, false);
    }
}
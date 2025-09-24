// Arquivo: PontoRefeitorio/Views/RegistroPage.xaml.cs
using PontoRefeitorio.ViewModels;

namespace PontoRefeitorio.Views;

public partial class RegistroPage : ContentPage
{
    public RegistroPage()
    {
        InitializeComponent();
        BindingContext = new RegistroPageViewModel();
    }

    private async void RegistroButton_Clicked(object sender, EventArgs e)
    {
        // Navega para a página da câmera usando a rota registada.
        await Shell.Current.GoToAsync(nameof(MainPage));
    }
}
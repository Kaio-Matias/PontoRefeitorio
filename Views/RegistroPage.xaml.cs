// Arquivo: PontoRefeitorio/Views/RegistroPage.xaml.cs
using PontoRefeitorio.ViewModels;

namespace PontoRefeitorio.Views;

// Adicione a palavra 'partial' aqui
public partial class RegistroPage : ContentPage
{
    public RegistroPage()
    {
        InitializeComponent();
        BindingContext = new RegistroPageViewModel();
    }

    private async void RegistroButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MainPage));
    }
}
using PontoRefeitorio.ViewModels;

namespace PontoRefeitorio.Views;

public partial class RegistroPage : ContentPage
{
    public RegistroPage(RegistroPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void RegistroButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MainPage));
    }
}
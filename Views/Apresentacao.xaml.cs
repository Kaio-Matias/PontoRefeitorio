namespace PontoRefeitorio.Views;

public partial class Apresentacao : ContentPage
{
    public Apresentacao()
    {
        InitializeComponent();
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(3000); // Manter a splash por 3 segundos

        var token = await SecureStorage.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
        {
            // Se o utilizador já está logado, vai para a página de registo
            await Shell.Current.GoToAsync($"///{nameof(RegistroPage)}");
        }
        else
        {
            // Senão, vai para a página de login
            await Shell.Current.GoToAsync($"///{nameof(LoginPage)}");
        }
    }
}
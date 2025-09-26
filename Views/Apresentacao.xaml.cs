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
            // Se o utilizador j� est� logado, vai para a p�gina de registo
            await Shell.Current.GoToAsync($"///{nameof(RegistroPage)}");
        }
        else
        {
            // Sen�o, vai para a p�gina de login
            await Shell.Current.GoToAsync($"///{nameof(LoginPage)}");
        }
    }
}
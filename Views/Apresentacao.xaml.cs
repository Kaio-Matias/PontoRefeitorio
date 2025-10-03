using PontoRefeitorio.Services;

namespace PontoRefeitorio.Views;

public partial class Apresentacao : ContentPage
{
    private readonly AuthService _authService;

    public Apresentacao(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(3000);

        var token = await SecureStorage.GetAsync("auth_token");

        if (!string.IsNullOrEmpty(token) && await _authService.IsTokenValidAsync(token))
        {
            // CORREÇÃO: Navega para a rota da TabBar principal.
            await Shell.Current.GoToAsync($"//MainApp/{nameof(RegistroPage)}");
        }
        else
        {
            SecureStorage.Remove("auth_token");
            // CORREÇÃO: Navega para a rota de Login.
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}
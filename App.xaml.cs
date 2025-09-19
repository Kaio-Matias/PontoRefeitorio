using PontoRefeitorio.Services;
using PontoRefeitorio.Views;

namespace PontoRefeitorio;

public partial class App : Application
{
    private readonly AuthService _authService;

    public App(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
        MainPage = new AppShell();
    }

    protected override async void OnStart()
    {
        // Verifica se já existe um token ao iniciar o app
        var token = await _authService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            // Se houver token, vai direto para a página principal
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
        else
        {
            // Caso contrário, vai para a página de login
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}

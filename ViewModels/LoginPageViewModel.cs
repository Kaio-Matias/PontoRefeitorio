using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PontoRefeitorio.Models;
using PontoRefeitorio.Services;
using PontoRefeitorio.Views;

namespace PontoRefeitorio.ViewModels
{
    public partial class LoginPageViewModel : ObservableObject
    {
        [ObservableProperty] private string username;
        [ObservableProperty] private string senha;
        [ObservableProperty] private string errorMessage;
        private readonly AuthService _authService;

        public LoginPageViewModel(AuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task SubmitLogin()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Senha))
            {
                ErrorMessage = "Por favor, preencha o utilizador e a senha.";
                return;
            }

            var loginRequest = new LoginRequest { Username = this.Username, Password = this.Senha };
            var response = await _authService.LoginAsync(loginRequest);

            if (response != null && !string.IsNullOrEmpty(response.Token))
            {
                ErrorMessage = string.Empty;
                // CORREÇÃO: Navega para a rota da TabBar principal.
                await Shell.Current.GoToAsync($"//MainApp/{nameof(RegistroPage)}");
            }
            else
            {
                ErrorMessage = response?.Message ?? "Falha no login.";
            }
        }
    }
}
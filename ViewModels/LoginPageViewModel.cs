using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PontoRefeitorio.Models;
using PontoRefeitorio.Services;
using PontoRefeitorio.Views;

namespace PontoRefeitorio.ViewModels
{
    public partial class LoginPageViewModel : ObservableObject
    {
        [ObservableProperty] private string email;
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
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Senha))
            {
                ErrorMessage = "Por favor, preencha o email e a senha.";
                return;
            }

            var loginRequest = new LoginRequest { Email = this.Email, Senha = this.Senha };
            var response = await _authService.LoginAsync(loginRequest);

            if (response != null && !string.IsNullOrEmpty(response.Token))
            {
                ErrorMessage = string.Empty;
                await Shell.Current.GoToAsync($"///{nameof(RegistroPage)}");
            }
            else
            {
                ErrorMessage = response?.Message ?? "Falha no login.";
            }
        }
    }
}
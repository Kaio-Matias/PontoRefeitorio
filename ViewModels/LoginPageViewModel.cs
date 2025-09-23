// Arquivo: PontoRefeitorio/ViewModels/LoginPageViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PontoRefeitorio.Models;
using PontoRefeitorio.Services;

namespace PontoRefeitorio.ViewModels
{
    public partial class LoginPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _senha;

        [ObservableProperty]
        private string _errorMessage;

        private readonly AuthService _authService;

        // **RESTAURADO:**
        // Voltamos para a injeção de dependência padrão, que é a melhor prática.
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
                Application.Current.MainPage = new AppShell();
            }
            else
            {
                ErrorMessage = response?.Message ?? "Falha no login.";
            }
        }
    }
}
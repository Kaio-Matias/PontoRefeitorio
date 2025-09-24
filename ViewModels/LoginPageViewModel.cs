// Arquivo: PontoRefeitorio/ViewModels/LoginPageViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PontoRefeitorio.Models;
using PontoRefeitorio.Services;
using PontoRefeitorio.Views;

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

                // ==================================================================
                // INÍCIO DA CORREÇÃO
                // ==================================================================
                // Navega para a página de registro de ponto como a nova página raiz.
                // O prefixo "//" limpa a pilha de navegação. Isso agora funciona
                // porque RegistroPage está definido na TabBar do AppShell.xaml.
                await Shell.Current.GoToAsync($"//{nameof(RegistroPage)}");
                // ==================================================================
                // FIM DA CORREÇÃO
                // ==================================================================
            }
            else
            {
                ErrorMessage = response?.Message ?? "Falha no login.";
            }
        }
    }
}
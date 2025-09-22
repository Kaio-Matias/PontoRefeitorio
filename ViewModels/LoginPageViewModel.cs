// PontoRefeitorio/ViewModels/LoginPageViewModel.cs

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PontoRefeitorio.Services;
using PontoRefeitorio.Views;
using System.Diagnostics;

namespace PontoRefeitorio.ViewModels
{
    public partial class LoginPageViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _senha;

        public LoginPageViewModel(AuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Senha))
            {
                await Shell.Current.DisplayAlert("Erro", "Por favor, preencha o email e a senha.", "OK");
                return;
            }

            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                bool success = await _authService.LoginAsync(Email, Senha);

                if (success)
                {
                    await Shell.Current.GoToAsync(nameof(MainPage));
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro de Login", "Email ou senha inválidos.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Ocorreu um erro ao tentar fazer login: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task BiometricsLoginAsync()
        {
            // A lógica para login com biometria será implementada aqui no futuro.
            await Shell.Current.DisplayAlert("Biometria", "Funcionalidade de login com biometria ainda não implementada.", "OK");
        }
    }
}
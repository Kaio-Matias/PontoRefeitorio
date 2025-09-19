// PontoRefeitorio/ViewModels/LoginPageViewModel.cs

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PontoRefeitorio.Services;
using PontoRefeitorio.Views;

namespace PontoRefeitorio.ViewModels
{
    public partial class LoginPageViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string _email;

        // ==================================================================
        // INÍCIO DA CORREÇÃO
        // ==================================================================
        // A propriedade "Senha" que estava faltando foi adicionada.
        [ObservableProperty]
        private string _senha;
        // ==================================================================
        // FIM DA CORREÇÃO
        // ==================================================================

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
                    // Navega para a página principal após o login bem-sucedido
                    await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
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
    }
}
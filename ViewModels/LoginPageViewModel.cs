using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PontoRefeitorio.Services;
using PontoRefeitorio.Views;

namespace PontoRefeitorio.ViewModels
{
    public partial class LoginPageViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private readonly AuthService _authService;

        [ObservableProperty]
        string email;

        [ObservableProperty]
        string password;

        public LoginPageViewModel(ApiService apiService, AuthService authService)
        {
            _apiService = apiService;
            _authService = authService;
            Title = "Login";
        }

        [RelayCommand]
        async Task LoginAsync()
        {
            if (IsBusy) return;
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Erro", "Por favor, preencha o email e a senha.", "OK");
                return;
            }

            try
            {
                IsBusy = true;
                var loginResponse = await _apiService.LoginAsync(Email, Password);
                if (!string.IsNullOrEmpty(loginResponse?.Token))
                {
                    await _authService.SetTokenAsync(loginResponse.Token);

                    // Navega para a página principal após o login
                    await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", "Token não recebido. Tente novamente.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro de Login", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}

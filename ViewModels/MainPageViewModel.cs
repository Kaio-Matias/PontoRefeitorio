// PontoRefeitorio/ViewModels/MainPageViewModel.cs

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PontoRefeitorio.Models; // Garanta que este using está presente
using PontoRefeitorio.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PontoRefeitorio.ViewModels
{
    public partial class MainPageViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private readonly AuthService _authService;
        private string takenPhotoPath;

        [ObservableProperty]
        private string colaboradorId;

        public MainPageViewModel(ApiService apiService, AuthService authService)
        {
            _apiService = apiService;
            _authService = authService;
        }

        [RelayCommand]
        private async Task TakePhotoAsync()
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null)
                {
                    takenPhotoPath = photo.FullPath;
                    await Shell.Current.DisplayAlert("Sucesso", $"Foto capturada!", "OK");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Erro", "Captura de foto não é suportada.", "OK");
            }
        }

        [RelayCommand]
        private async Task RegistrarPontoAsync()
        {
            if (string.IsNullOrEmpty(ColaboradorId) || string.IsNullOrEmpty(takenPhotoPath))
            {
                await Shell.Current.DisplayAlert("Atenção", "Informe o ID e tire a foto para registrar.", "OK");
                return;
            }

            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var token = await _authService.GetTokenAsync();
                var response = await _apiService.RegistrarPontoAsync(token, ColaboradorId, takenPhotoPath);

                // Verificando a propriedade "Sucesso"
                if (response != null && response.Sucesso)
                {
                    await Shell.Current.DisplayAlert("Sucesso", response.Message, "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Falha", response?.Message ?? "Não foi possível registrar o ponto.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"--> Erro ao registrar ponto: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", "Ocorreu um erro inesperado.", "OK");
            }
            finally
            {
                IsBusy = false;
                ColaboradorId = string.Empty;
                takenPhotoPath = null;
            }
        }
    }
}
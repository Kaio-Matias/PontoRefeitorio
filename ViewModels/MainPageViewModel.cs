using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PontoRefeitorio.Services;

namespace PontoRefeitorio.ViewModels
{
    public partial class MainPageViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        string colaboradorId;

        [ObservableProperty]
        string statusMessage;

        [ObservableProperty]
        Color statusColor;

        [ObservableProperty]
        ImageSource capturedImageSource;

        public MainPageViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Registrar Refeição";
            StatusColor = Colors.Transparent;
        }

        [RelayCommand]
        async Task CaptureAndRegisterAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(ColaboradorId))
            {
                await Shell.Current.DisplayAlert("Atenção", "Por favor, digite o ID do colaborador.", "OK");
                return;
            }

            try
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await Shell.Current.DisplayAlert("Não Suportado", "A captura de fotos não é suportada neste dispositivo.", "OK");
                    return;
                }

                IsBusy = true;
                StatusMessage = "Abrindo a câmera...";
                StatusColor = Colors.Orange;

                FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo == null)
                {
                    StatusMessage = "Captura cancelada.";
                    return;
                }

                // Exibe a foto na tela
                var stream = await photo.OpenReadAsync();
                CapturedImageSource = ImageSource.FromStream(() => stream);

                StatusMessage = "Enviando para verificação...";

                var response = await _apiService.RegistrarPontoAsync(ColaboradorId, photo.FullPath);

                StatusMessage = $"{response.Message} (Confiança: {response.Confidence:P2})";
                StatusColor = Colors.Green;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Falha no registro: {ex.Message}";
                StatusColor = Colors.Red;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}

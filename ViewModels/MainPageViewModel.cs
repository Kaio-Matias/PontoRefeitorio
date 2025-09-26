using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PontoRefeitorio.Services;
using PontoRefeitorio.Models;
using System.IO;
using System.Threading.Tasks;

namespace PontoRefeitorio.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly ApiService _apiService;
        private CameraView _cameraView;

        [ObservableProperty] private bool showResultOverlay;
        [ObservableProperty] private string colaboradorNome;
        [ObservableProperty] private ImageSource colaboradorFoto;
        [ObservableProperty] private string resultMessage;
        [ObservableProperty] private Color resultBackgroundColor;
        [ObservableProperty] private bool isBusy;
        public bool IsButtonEnabled => !IsBusy;

        public MainPageViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public void InitializeCamera(CameraView cameraView) => _cameraView = cameraView;

        [RelayCommand]
        private async Task RegistrarPonto()
        {
            if (IsBusy || _cameraView == null) return;
            IsBusy = true;
            OnPropertyChanged(nameof(IsButtonEnabled));

            try
            {
                var photoStream = await _cameraView.CaptureImage(CancellationToken.None);
                if (photoStream != null)
                {
                    using var memoryStream = new MemoryStream();
                    await photoStream.CopyToAsync(memoryStream);
                    var photoBytes = memoryStream.ToArray();
                    var response = await _apiService.RegistrarPonto(photoBytes);

                    if (response != null && response.Sucesso)
                    {
                        ColaboradorNome = response.Nome;
                        ResultMessage = response.Mensagem;
                        ResultBackgroundColor = Colors.Green;
                        if (!string.IsNullOrEmpty(response.FotoBase64))
                        {
                            ColaboradorFoto = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(response.FotoBase64)));
                        }
                    }
                    else
                    {
                        HandleFailure(response?.Mensagem ?? "Rosto não reconhecido.");
                    }
                }
                else
                {
                    HandleFailure("Não foi possível capturar a foto.");
                }
            }
            catch (Exception ex)
            {
                HandleFailure($"Erro crítico: {ex.Message}");
            }
            finally
            {
                ShowResultOverlay = true;
                await Task.Delay(4000);
                ShowResultOverlay = false;
                IsBusy = false;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        private void HandleFailure(string message)
        {
            ColaboradorNome = string.Empty;
            ColaboradorFoto = "dotnet_bot.png"; // Imagem de placeholder
            ResultMessage = message;
            ResultBackgroundColor = Colors.Red;
        }
    }
}
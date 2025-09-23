// Arquivo: PontoRefeitorio/ViewModels/MainPageViewModel.cs

using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PontoRefeitorio.Services;

namespace PontoRefeitorio.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly ApiService _apiService;
        private CameraView _cameraView;

        [ObservableProperty]
        private bool _showResultOverlay;

        [ObservableProperty]
        private string _colaboradorNome;

        [ObservableProperty]
        private ImageSource _colaboradorFoto;

        [ObservableProperty]
        private string _resultMessage;

        [ObservableProperty]
        private Color _resultBackgroundColor;

        [ObservableProperty]
        private bool _isBusy;

        public bool IsButtonEnabled => !IsBusy;

        public MainPageViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public void InitializeCamera(CameraView cameraView)
        {
            _cameraView = cameraView;
        }

        [RelayCommand]
        private async Task RegistrarPonto()
        {
            if (IsBusy) return;
            IsBusy = true;
            OnPropertyChanged(nameof(IsButtonEnabled));

            try
            {
                // Esta é a chamada correta que você descobriu!
                var photoStream = await _cameraView?.CaptureAsync();

                if (photoStream != null)
                {
                    using var memoryStream = new MemoryStream();
                    await photoStream.CopyToAsync(memoryStream);
                    var photoBytes = memoryStream.ToArray();

                    var response = await _apiService.RegistrarPonto(photoBytes);

                    if (response != null && response.Sucesso)
                    {
                        ColaboradorNome = response.Nome;
                        ColaboradorFoto = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(response.FotoBase64)));
                        ResultMessage = "Bem-vindo(a)!";
                        ResultBackgroundColor = Colors.Green;
                    }
                    else
                    {
                        ColaboradorNome = string.Empty;
                        ColaboradorFoto = "dotnet_bot.png";
                        ResultMessage = response?.Mensagem ?? "Rosto não reconhecido.";
                        ResultBackgroundColor = Colors.Red;
                    }
                }
                else
                {
                    ColaboradorNome = string.Empty;
                    ColaboradorFoto = "dotnet_bot.png";
                    ResultMessage = "Não foi possível capturar a foto.";
                    ResultBackgroundColor = Colors.Red;
                }
            }
            catch (Exception ex)
            {
                ColaboradorNome = string.Empty;
                ColaboradorFoto = "dotnet_bot.png";
                ResultMessage = $"Erro: {ex.Message}";
                ResultBackgroundColor = Colors.Red;
            }
            finally
            {
                ShowResultOverlay = true;
                await Task.Delay(5000);
                ShowResultOverlay = false;

                IsBusy = false;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }
    }
}
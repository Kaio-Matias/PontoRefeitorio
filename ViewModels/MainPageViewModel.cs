// Arquivo: PontoRefeitorio/ViewModels/MainPageViewModel.cs
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PontoRefeitorio.Services;
using PontoRefeitorio.Models;
using Microsoft.Maui.Media; // Necessário para ImageFormat

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
            if (IsBusy || _cameraView == null) return;

            IsBusy = true;
            OnPropertyChanged(nameof(IsButtonEnabled));

            try
            {
                await Task.Delay(500); // Pequeno atraso para garantir que a câmera está pronta

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
                ResultMessage = $"Erro crítico: {ex.Message}";
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

        // ==================================================================
        // INÍCIO DA NOVA FUNCIONALIDADE
        // ==================================================================
        [RelayCommand]
        private async Task SavePhoto()
        {
            if (IsBusy || _cameraView == null) return;

            IsBusy = true;
            OnPropertyChanged(nameof(IsButtonEnabled));
            string photoPath = string.Empty;

            try
            {
                // Solicita permissão para escrever no armazenamento
                var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
                if (status != PermissionStatus.Granted)
                {
                    await Shell.Current.DisplayAlert("Permissão Negada", "Não é possível salvar a foto sem permissão de armazenamento.", "OK");
                    return;
                }

                await Task.Delay(500); // Garante que a câmera está pronta
                var photoStream = await _cameraView.CaptureImage(CancellationToken.None);

                if (photoStream != null)
                {
                    // Define o nome do arquivo e o caminho onde será salvo
                    string fileName = $"PontoRefeitorio_Teste_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.jpg";
                    photoPath = Path.Combine(FileSystem.CacheDirectory, fileName);

                    // Lê o stream da foto e salva no arquivo
                    using (var fileStream = new FileStream(photoPath, FileMode.Create, FileAccess.Write))
                    {
                        await photoStream.CopyToAsync(fileStream);
                    }

                    await Shell.Current.DisplayAlert("Sucesso", $"Foto salva com sucesso! Você pode encontrá-la em:\n\n{photoPath}", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", "Não foi possível capturar a foto.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro Crítico", $"Ocorreu um erro ao salvar a foto: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }
        // ==================================================================
        // FIM DA NOVA FUNCIONALIDADE
        // ==================================================================
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PontoRefeitorio.Models;
using PontoRefeitorio.Services;
using System.Diagnostics;

namespace PontoRefeitorio.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly ApiService _apiService;
        private IDispatcherTimer _countdownTimer;
        private Action _takePhotoAction;
        private CancellationTokenSource _registrationCts;

        [ObservableProperty]
        private bool _showInitialLayout = true;

        [ObservableProperty]
        private bool _showCountdown = false;

        [ObservableProperty]
        private bool _showPopup = false;

        [ObservableProperty]
        private int _countdownValue = 10;

        [ObservableProperty]
        private ImageSource _capturedImageSource;

        [ObservableProperty]
        private string _colaboradorNome;

        [ObservableProperty]
        private string _resultMessage;

        [ObservableProperty]
        private Color _resultBackgroundColor;

        [ObservableProperty]
        private bool _isBusy = false;
        private byte[] _capturedImageBytes;

        public MainPageViewModel(ApiService apiService)
        {
            _apiService = apiService;
            _registrationCts = new CancellationTokenSource();
            _countdownTimer = Application.Current.Dispatcher.CreateTimer();
            _countdownTimer.Interval = TimeSpan.FromSeconds(1);
            _countdownTimer.Tick += OnTimerTick;
        }

        public void SetTakePhotoAction(Action takePhotoAction)
        {
            _takePhotoAction = takePhotoAction;
        }

        [RelayCommand]
        private void StartCaptureProcess()
        {
            if (IsBusy) return;

            ShowInitialLayout = false;
            ShowCountdown = true;
            CountdownValue = 10;
            _countdownTimer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            CountdownValue--;
            if (CountdownValue <= 0)
            {
                _countdownTimer.Stop();
                _takePhotoAction?.Invoke();
            }
        }

        public async Task ProcessCapturedImage(byte[] imageBytes)
        {
            _capturedImageBytes = imageBytes;
            CapturedImageSource = ImageSource.FromStream(() => new MemoryStream(_capturedImageBytes));

            ShowCountdown = false;
            ShowPopup = true;

            await RegisterPoint();
        }

        private async Task RegisterPoint()
        {
            if (_capturedImageBytes == null || IsBusy) return;

            IsBusy = true;
            ResultMessage = "Registrando...";
            ResultBackgroundColor = Colors.Transparent;
            _registrationCts = new CancellationTokenSource();

            try
            {
                var response = await _apiService.RegistrarPonto(_capturedImageBytes, _registrationCts.Token);

                if (_registrationCts.IsCancellationRequested)
                {
                    HandleResult(false, "Registro cancelado.");
                    await Task.Delay(2000);
                }
                else
                {
                    HandleResult(response.Sucesso, response.Mensagem, response.Nome, response.FotoBase64);
                }
            }
            catch (OperationCanceledException)
            {
                HandleResult(false, "Registro cancelado.");
                await Task.Delay(2000);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro crítico ao registrar ponto: {ex.Message}");
                HandleResult(false, "Falha na conexão com o servidor.");
            }
            finally
            {
                IsBusy = false;
                await Task.Delay(4000);
                ResetState();
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            if (IsBusy)
            {
                _registrationCts?.Cancel();
            }
            else
            {
                ResetState();
            }
        }

        private void HandleResult(bool success, string message, string nome = null, string photoBase64 = null)
        {
            ResultMessage = message;
            ColaboradorNome = nome ?? string.Empty;
            ResultBackgroundColor = success ? Colors.Green : Colors.Red;

            if (success && !string.IsNullOrEmpty(photoBase64))
            {
                CapturedImageSource = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(photoBase64)));
            }
        }

        private void ResetState()
        {
            ShowPopup = false;
            ShowInitialLayout = true;
            ShowCountdown = false;
            _capturedImageBytes = null;
            _countdownTimer.Stop();
        }
    }
}
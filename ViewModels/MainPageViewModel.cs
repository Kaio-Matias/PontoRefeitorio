using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PontoRefeitorio.Services;
using System.Timers;
using Microsoft.Maui.Dispatching;

namespace PontoRefeitorio.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        // Temporizadores
        private System.Timers.Timer _timerCaptura;
        private System.Timers.Timer _timerConfirmacao;

        // Ação para disparar a captura na View (Code Behind)
        public Action RequestCaptureAction { get; set; }

        // --- PROPRIEDADES DE ESTADO VISUAL ---
        [ObservableProperty] private bool _showInitialLayout = true; // Tela inicial (se houver)
        [ObservableProperty] private bool _showCountdown = false;    // Contagem 3, 2, 1
        [ObservableProperty] private bool _showPreview = false;      // Foto capturada + Botões
        [ObservableProperty] private bool _showResultPopup = false;  // Resultado da API
        [ObservableProperty] private bool _isBusy = false;           // Loading

        // --- DADOS PARA A UI ---
        [ObservableProperty] private string _countdownText;          // Texto "3", "2"...
        [ObservableProperty] private string _confirmationButtonText; // "CONFIRMAR (59s)"
        [ObservableProperty] private ImageSource _previewImageSource; // A foto para validar (na tela de preview)
        [ObservableProperty] private ImageSource _resultImageSource;  // A foto no resultado (no popup final)

        // Propriedades para o Popup de Resultado
        [ObservableProperty] private string _colaboradorNome;
        [ObservableProperty] private string _resultMessage;
        [ObservableProperty] private Color _resultBackgroundColor;

        // Armazena os bytes da foto temporariamente antes de enviar
        private byte[] _fotoBytesTemporaria;
        private int _segundosConfirmacao;

        public MainPageViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        // 1. INICIA O PROCESSO (Chamado ao abrir a tela ou clicar no botão)
        [RelayCommand]
        public void StartCaptureProcess()
        {
            ResetState();
            ShowInitialLayout = false;
            ShowCountdown = true;

            // Inicia contagem de 3 segundos para captura
            var segundos = 3;
            CountdownText = segundos.ToString();

            _timerCaptura = new System.Timers.Timer(1000);
            _timerCaptura.Elapsed += (s, e) =>
            {
                segundos--;

                // Atualiza UI na MainThread
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (segundos > 0)
                    {
                        CountdownText = segundos.ToString();
                    }
                    else
                    {
                        _timerCaptura.Stop();
                        CountdownText = "";
                        ShowCountdown = false;

                        // Solicita à View (Code Behind) que tire a foto agora
                        RequestCaptureAction?.Invoke();
                    }
                });
            };
            _timerCaptura.Start();
        }

        // 2. RECEBE A IMAGEM CAPTURADA (Chamado pelo Code Behind)
        public void SetImageForConfirmation(byte[] fotoBytes)
        {
            _fotoBytesTemporaria = fotoBytes;

            // Cria o ImageSource a partir dos bytes
            PreviewImageSource = ImageSource.FromStream(() => new MemoryStream(fotoBytes));

            // Muda o estado visual para mostrar a confirmação
            ShowPreview = true;

            // Inicia o Timer de 60s para confirmação/cancelamento
            StartConfirmationTimer();
        }

        private void StartConfirmationTimer()
        {
            _segundosConfirmacao = 60;
            ConfirmationButtonText = $"CONFIRMAR ({_segundosConfirmacao}s)";

            _timerConfirmacao = new System.Timers.Timer(1000);
            _timerConfirmacao.Elapsed += (s, e) =>
            {
                _segundosConfirmacao--;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (_segundosConfirmacao > 0)
                    {
                        ConfirmationButtonText = $"CONFIRMAR ({_segundosConfirmacao}s)";
                    }
                    else
                    {
                        // Tempo esgotou: Cancela automaticamente
                        _timerConfirmacao.Stop();
                        CancelAndGoBack();
                    }
                });
            };
            _timerConfirmacao.Start();
        }

        // 3. ENVIA PARA API (Chamado pelo Botão Confirmar)
        [RelayCommand]
        public async Task ConfirmAndSend()
        {
            // Para o timer de 60s
            _timerConfirmacao?.Stop();

            // Atualiza UI para estado de "Enviando"
            ShowPreview = false;
            ShowResultPopup = true;
            IsBusy = true;
            ResultMessage = "Processando...";
            ResultImageSource = PreviewImageSource; // Mantém a foto visível no popup
            ResultBackgroundColor = Colors.Gray;

            try
            {
                // CORREÇÃO APLICADA: Chama 'RegistrarPonto' passando CancellationToken
                var resultado = await _apiService.RegistrarPonto(_fotoBytesTemporaria, CancellationToken.None);

                if (resultado != null && resultado.Sucesso)
                {
                    // SUCESSO
                    // CORREÇÃO APLICADA: Usa 'resultado.Nome' em vez de 'NomeColaborador'
                    ColaboradorNome = "Olá, " + (resultado.Nome ?? "Colaborador");
                    ResultMessage = resultado.Mensagem;
                    ResultBackgroundColor = Colors.Green;

                    // Espera 3s e fecha
                    await Task.Delay(3000);
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    // FALHA (Não reconheceu ou erro de validação)
                    ColaboradorNome = "Atenção";
                    ResultMessage = resultado?.Mensagem ?? "Erro desconhecido";
                    ResultBackgroundColor = Colors.Red;

                    // Espera 4s e reinicia o processo de captura
                    await Task.Delay(4000);
                    ResetState();
                    StartCaptureProcess();
                }
            }
            catch (Exception)
            {
                // ERRO DE CONEXÃO
                ColaboradorNome = "Erro";
                ResultMessage = "Falha de conexão com o servidor";
                ResultBackgroundColor = Colors.Orange;

                await Task.Delay(3000);
                await Shell.Current.GoToAsync("..");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // 4. CANCELA (Chamado pelo Botão X ou Timeout)
        [RelayCommand]
        public void CancelAndGoBack()
        {
            StopAllTimers();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync("..");
            });
        }

        private void StopAllTimers()
        {
            _timerCaptura?.Stop();
            _timerConfirmacao?.Stop();
        }

        private void ResetState()
        {
            StopAllTimers();
            ShowInitialLayout = true;
            ShowCountdown = false;
            ShowPreview = false;
            ShowResultPopup = false;
            IsBusy = false;
            _fotoBytesTemporaria = null;
        }

        // Método auxiliar exigido pela interface anterior, deixado vazio se não usar som
        public void SetMediaElement(object mediaElement) { }

        // Método auxiliar exigido pelo code-behind antigo
        public void SetTakePhotoAction(Action action) { RequestCaptureAction = action; }

        // Método auxiliar exigido pelo code-behind antigo
        public Task ProcessCapturedImage(byte[] image)
        {
            SetImageForConfirmation(image);
            return Task.CompletedTask;
        }
    }
}
using CommunityToolkit.Maui.Core;
using PontoRefeitorio.ViewModels;
#if ANDROID
using Android.Graphics;
#endif

namespace PontoRefeitorio.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly ICameraProvider _cameraProvider;
        private readonly MainPageViewModel _viewModel;

        public MainPage(MainPageViewModel viewModel, ICameraProvider cameraProvider)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
            _cameraProvider = cameraProvider;

            // Configura a ação que o ViewModel chama quando o timer de 3s acaba
            _viewModel.RequestCaptureAction = async () => await CaptureAndProcess();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RequestCameraPermission();
            await SetCamera();

            // Inicia o fluxo automaticamente assim que a tela abre
            if (_viewModel.StartCaptureProcessCommand.CanExecute(null))
            {
                _viewModel.StartCaptureProcessCommand.Execute(null);
            }
        }

        private async Task CaptureAndProcess()
        {
            try
            {
                // Captura a imagem usando o Toolkit
                var photoStream = await cameraView.CaptureImage(CancellationToken.None);

                if (photoStream != null)
                {
                    using var memoryStream = new MemoryStream();
                    await photoStream.CopyToAsync(memoryStream);
                    var imageBytes = memoryStream.ToArray();

                    // Processamento de rotação (Correção para Android Front Camera)
                    var processedImage = await ResizeAndRotateImage(imageBytes, 600, 800);

                    // IMPORTANTE: Envia para o ViewModel iniciar o Timer de 60s e mostrar Preview
                    _viewModel.SetImageForConfirmation(processedImage);
                }
                else
                {
                    // Falha silenciosa ou retry
                    await DisplayAlert("Erro", "Não foi possível capturar a imagem.", "OK");
                    await Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro na captura: {ex.Message}", "OK");
                await Navigation.PopAsync();
            }
        }

        private async Task SetCamera()
        {
            await _cameraProvider.RefreshAvailableCameras(CancellationToken.None);
            var cameras = _cameraProvider.AvailableCameras;
            if (cameras.Count > 0)
            {
                // Tenta pegar a câmera frontal, senão pega a primeira disponível
                var frontCamera = cameras.FirstOrDefault(c => c.Position == CameraPosition.Front);
                cameraView.SelectedCamera = frontCamera ?? cameras.First();
            }
        }

        private async Task RequestCameraPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Erro", "Permissão de câmera necessária.", "OK");
                await Navigation.PopAsync();
            }
        }

        /// <summary>
        /// Redimensiona e rotaciona a imagem para o formato retrato (portrait)
        /// </summary>
        private async Task<byte[]> ResizeAndRotateImage(byte[] imageData, float width, float height)
        {
#if ANDROID
            try
            {
                var originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
                var matrix = new Matrix();

                // Rotação padrão para câmera frontal Android (270 ou -90)
                matrix.PostRotate(270);

                var rotatedBitmap = Bitmap.CreateBitmap(originalImage, 0, 0, originalImage.Width, originalImage.Height, matrix, true);
                var scaledBitmap = Bitmap.CreateScaledBitmap(rotatedBitmap, (int)width, (int)height, true);

                using (MemoryStream ms = new MemoryStream())
                {
                    scaledBitmap.Compress(Bitmap.CompressFormat.Jpeg, 85, ms);
                    originalImage.Recycle();
                    rotatedBitmap.Recycle();
                    scaledBitmap.Recycle();
                    return ms.ToArray();
                }
            }
            catch
            {
                return imageData;
            }
#else
            // Para iOS/Windows retorna a imagem original (ou implemente lógica específica se necessário)
            return await Task.FromResult(imageData);
#endif
        }
    }
}
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
            _viewModel.SetMediaElement(mediaElement);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RequestCameraPermission();
            await SetCamera();

            if (BindingContext is MainPageViewModel viewModel)
            {
                viewModel.SetTakePhotoAction(async () =>
                {
                    var photoStream = await cameraView.CaptureImage(CancellationToken.None);
                    if (photoStream != null)
                    {
                        using var memoryStream = new MemoryStream();
                        await photoStream.CopyToAsync(memoryStream);
                        var imageBytes = memoryStream.ToArray();

                        // Redimensiona e rotaciona a imagem antes de enviar para a API
                        var processedImage = await ResizeAndRotateImage(imageBytes, 600, 800);
                        await viewModel.ProcessCapturedImage(processedImage);
                    }
                });

                // Inicia o processo de captura assim que a página aparece
                if (viewModel.ShowInitialLayout)
                {
                    viewModel.StartCaptureProcessCommand.Execute(null);
                }
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

                // AJUSTE: A rotação correta para a câmera frontal na maioria dos dispositivos Android
                // é de 270 graus (ou -90) para corrigir a orientação do sensor.
                matrix.PostRotate(270);

                // Cria o bitmap rotacionado a partir da imagem original
                var rotatedBitmap = Bitmap.CreateBitmap(originalImage, 0, 0, originalImage.Width, originalImage.Height, matrix, true);

                // Redimensiona o bitmap já rotacionado
                var scaledBitmap = Bitmap.CreateScaledBitmap(rotatedBitmap, (int)width, (int)height, true);

                using (MemoryStream ms = new MemoryStream())
                {
                    // Comprime a imagem final para garantir um tamanho de arquivo menor
                    scaledBitmap.Compress(Bitmap.CompressFormat.Jpeg, 85, ms); // Qualidade de 85%

                    // Libera a memória dos bitmaps
                    originalImage.Recycle();
                    rotatedBitmap.Recycle();
                    scaledBitmap.Recycle();

                    return ms.ToArray();
                }
            }
            catch (Exception)
            {
                // Em caso de erro, retorna a imagem original
                return imageData;
            }

#else
            // Para outras plataformas, retorna a imagem original por enquanto.
            return await Task.FromResult(imageData);
#endif
        }


        private async Task SetCamera()
        {
            await _cameraProvider.RefreshAvailableCameras(CancellationToken.None);
            var cameras = _cameraProvider.AvailableCameras;
            if (cameras.Count > 0)
            {
                var frontCamera = cameras.FirstOrDefault(c => c.Position == CameraPosition.Front);
                cameraView.SelectedCamera = frontCamera ?? cameras.First();
                if (cameraView.SelectedCamera != null && cameraView.SelectedCamera.MaximumZoomFactor > 1f)
                {
                    cameraView.ZoomFactor = 1.5f;
                }
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
                await DisplayAlert("Permissão Necessária", "A permissão da câmara é necessária.", "OK");
            }
        }
    }
}
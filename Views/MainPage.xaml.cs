using CommunityToolkit.Maui.Core;
using PontoRefeitorio.ViewModels;

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
                        await viewModel.ProcessCapturedImage(imageBytes);
                    }
                });

                viewModel.StartCaptureProcessCommand.Execute(null); // inicia automaticamente
            }
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

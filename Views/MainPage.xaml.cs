using CommunityToolkit.Maui.Core;
using PontoRefeitorio.ViewModels;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Linq;
using System.Threading;

namespace PontoRefeitorio.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly ICameraProvider _cameraProvider;

        public MainPage(MainPageViewModel viewModel, ICameraProvider cameraProvider)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _cameraProvider = cameraProvider;
            // A inicialização da câmera foi movida para OnAppearing para maior estabilidade
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RequestCameraPermission();
            await SetCamera();

            // CORREÇÃO: Passa a referência da cameraView para o ViewModel aqui,
            // depois que a página já está visível e a câmera sendo configurada.
            if (BindingContext is MainPageViewModel viewModel)
            {
                viewModel.InitializeCamera(cameraView);
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

                // Garante que a câmera foi selecionada antes de aplicar o zoom.
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
                await DisplayAlert("Permissão Necessária", "A permissão da câmera é necessária para o reconhecimento facial.", "OK");
            }
        }
    }
}

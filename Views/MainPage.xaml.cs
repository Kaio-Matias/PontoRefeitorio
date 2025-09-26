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
            // A inicializa��o da c�mera foi movida para OnAppearing para maior estabilidade
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RequestCameraPermission();
            await SetCamera();

            // CORRE��O: Passa a refer�ncia da cameraView para o ViewModel aqui,
            // depois que a p�gina j� est� vis�vel e a c�mera sendo configurada.
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

                // Garante que a c�mera foi selecionada antes de aplicar o zoom.
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
                await DisplayAlert("Permiss�o Necess�ria", "A permiss�o da c�mera � necess�ria para o reconhecimento facial.", "OK");
            }
        }
    }
}

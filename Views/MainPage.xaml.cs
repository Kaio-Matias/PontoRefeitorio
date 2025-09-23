using CommunityToolkit.Maui.Core;
using PontoRefeitorio.ViewModels;

namespace PontoRefeitorio.Views;

public partial class MainPage : ContentPage
{
    private readonly ICameraProvider _cameraProvider;

    // O construtor recebe o ICameraProvider por injeção de dependência
    public MainPage(MainPageViewModel viewModel, ICameraProvider cameraProvider)
    {
        InitializeComponent();
        BindingContext = viewModel;
        viewModel.InitializeCamera(cameraView);
        _cameraProvider = cameraProvider; // Armazena a instância do provedor
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RequestCameraPermission();
        await SetCamera();
    }

    private async Task SetCamera()
    {
        // Usa o provedor para atualizar a lista de câmeras
        await _cameraProvider.RefreshAvailableCameras(CancellationToken.None);

        // Pega a lista de câmeras diretamente do provedor
        var cameras = _cameraProvider.AvailableCameras;

        if (cameras.Count > 0)
        {
            // Procura pela câmera frontal na lista do provedor
            var frontCamera = cameras.FirstOrDefault(c => c.Position == CameraPosition.Front);

            // Define a câmera selecionada na CameraView
            cameraView.SelectedCamera = frontCamera ?? cameras.First();

            // Garante que a câmera foi selecionada antes de aplicar o zoom.
            if (cameraView.SelectedCamera != null && cameraView.SelectedCamera.MaximumZoomFactor > 1f)
            {
                // Aplica o zoom inicial
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
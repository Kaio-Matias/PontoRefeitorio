using CommunityToolkit.Maui.Core;
using PontoRefeitorio.ViewModels;

namespace PontoRefeitorio.Views;

public partial class MainPage : ContentPage
{
    private readonly ICameraProvider _cameraProvider;

    // O construtor recebe o ICameraProvider por inje��o de depend�ncia
    public MainPage(MainPageViewModel viewModel, ICameraProvider cameraProvider)
    {
        InitializeComponent();
        BindingContext = viewModel;
        viewModel.InitializeCamera(cameraView);
        _cameraProvider = cameraProvider; // Armazena a inst�ncia do provedor
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RequestCameraPermission();
        await SetCamera();
    }

    private async Task SetCamera()
    {
        // Usa o provedor para atualizar a lista de c�meras
        await _cameraProvider.RefreshAvailableCameras(CancellationToken.None);

        // Pega a lista de c�meras diretamente do provedor
        var cameras = _cameraProvider.AvailableCameras;

        if (cameras.Count > 0)
        {
            // Procura pela c�mera frontal na lista do provedor
            var frontCamera = cameras.FirstOrDefault(c => c.Position == CameraPosition.Front);

            // Define a c�mera selecionada na CameraView
            cameraView.SelectedCamera = frontCamera ?? cameras.First();

            // Garante que a c�mera foi selecionada antes de aplicar o zoom.
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
            await DisplayAlert("Permiss�o Necess�ria", "A permiss�o da c�mera � necess�ria para o reconhecimento facial.", "OK");
        }
    }
}
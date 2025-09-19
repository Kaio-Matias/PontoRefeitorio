// PontoRefeitorio/MauiProgram.cs

using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using PontoRefeitorio.Services;
using PontoRefeitorio.ViewModels;
using PontoRefeitorio.Views;

namespace PontoRefeitorio;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitCamera()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // ==================================================================
        // INÍCIO DA CORREÇÃO
        // ==================================================================
        // Garantindo que todos os serviços, ViewModels e Telas
        // estejam corretamente registrados no sistema de injeção de dependência.

        // Services
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<ApiService>();

        // ViewModels (Transient é melhor para ViewModels de página)
        builder.Services.AddTransient<LoginPageViewModel>();
        builder.Services.AddTransient<MainPageViewModel>();

        // Views / Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<MainPage>();
        // ==================================================================
        // FIM DA CORREÇÃO
        // ==================================================================

        return builder.Build();
    }
}
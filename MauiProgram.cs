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
        // Services
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<ApiService>();

        // ViewModels
        builder.Services.AddSingleton<LoginPageViewModel>();
        builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddTransient<RegistroPageViewModel>();

        // Pages
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddTransient<MainPage>(); 
        builder.Services.AddTransient<Apresentacao>();
        builder.Services.AddTransient<RegistroPage>();

        return builder.Build();
    }
}
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using PontoRefeitorio.Views;
using PontoRefeitorio.ViewModels;
using PontoRefeitorio.Services;

namespace PontoRefeitorio
{
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
            // Registrando Serviços como Singleton (uma única instância para todo o app)
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<ApiService>();

            // Registrando Views e ViewModels
            // Views são transitórias (criadas sempre que navegamos para elas)
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<MainPage>();

            // ViewModels são transitórios também
            builder.Services.AddTransient<LoginPageViewModel>();
            builder.Services.AddTransient<MainPageViewModel>();
            return builder.Build();
        }
    }
}

using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using PontoRefeitorio.Services;
using PontoRefeitorio.ViewModels;
using PontoRefeitorio.Views;

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

            // ==================================================================
            // INÍCIO DA CORREÇÃO
            // ==================================================================
            // Registra o HttpClient como um singleton. Isso informa ao sistema
            // como criar um HttpClient sempre que um serviço (como o ApiService) precisar dele.
            builder.Services.AddSingleton<HttpClient>();

            // Registra os serviços, viewmodels e páginas para injeção de dependência
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<ApiService>();

            builder.Services.AddTransient<LoginPageViewModel>();
            builder.Services.AddTransient<LoginPage>();

            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<MainPage>();

            builder.Services.AddTransient<RegistroPageViewModel>();
            builder.Services.AddTransient<RegistroPage>();
            // ==================================================================
            // FIM DA CORREÇÃO
            // ==================================================================


            return builder.Build();
        }
    }
}

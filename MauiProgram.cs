using PontoRefeitorio.Services;
using PontoRefeitorio.ViewModels;
using PontoRefeitorio.Views;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace PontoRefeitorio
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkitCamera() // Necessário para o CameraView
                .UseMauiCommunityToolkit() // Necessário para o Base64 to Image converter
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            // REGISTO DE SERVIÇOS
            // Singletons garantem que haverá apenas uma instância destes serviços na aplicação.
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<ApiService>();

            // REGISTO DE VIEWS E VIEWMODELS
            // Transient para as páginas e ViewModels, para que sejam criados novos a cada navegação.
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<LoginPageViewModel>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainPageViewModel>();

            builder.Services.AddTransient<RegistroPage>();
            builder.Services.AddTransient<RegistroPageViewModel>();

            // NOVO: Registo da página Apresentacao para injeção de dependência.
            builder.Services.AddTransient<Apresentacao>();

            return builder.Build();
        }
    }
}
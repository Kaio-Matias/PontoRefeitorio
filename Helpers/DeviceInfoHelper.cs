// PontoRefeitorio/Helpers/DeviceInfoHelper.cs

namespace PontoRefeitorio.Helpers
{
    public static class DeviceInfoHelper
    {
        public static string GetDeviceIdentifier()
        {
            // Usa compilação condicional para obter o ID único de cada plataforma.
            // Esta é a maneira correta e recomendada no .NET MAUI.
#if ANDROID
            return Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
#elif IOS
            return UIKit.UIDevice.CurrentDevice.IdentifierForVendor.ToString();
#elif WINDOWS
            // Para outras plataformas como Windows, um GUID funciona como um identificador único.
            return System.Guid.NewGuid().ToString();
#else
            return System.Guid.NewGuid().ToString();
#endif
        }

        public static string GetDeviceName()
        {
            return $"{DeviceInfo.Current.Manufacturer} {DeviceInfo.Current.Model}";
        }
    }
}
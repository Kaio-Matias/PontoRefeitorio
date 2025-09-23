// Arquivo: PontoRefeitorio/Helpers/DeviceInfoHelper.cs

namespace PontoRefeitorio.Helpers
{
    public static class DeviceInfoHelper
    {
        public static string GetDeviceId()
        {
            // Gera um ID único e o armazena para que seja o mesmo em futuras execuções.
            var deviceId = Preferences.Get("device_id", string.Empty);
            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = Guid.NewGuid().ToString();
                Preferences.Set("device_id", deviceId);
            }
            return deviceId;
        }
    }
}
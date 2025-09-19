namespace PontoRefeitorio.Helpers
{
    public static class DeviceInfoHelper
    {
        private const string DeviceIdKey = "unique_device_id";

        public static async Task<string> GetDeviceIdentifierAsync()
        {
            try
            {
                string deviceId = await SecureStorage.GetAsync(DeviceIdKey);
                if (string.IsNullOrEmpty(deviceId))
                {
                    deviceId = Guid.NewGuid().ToString();
                    await SecureStorage.SetAsync(DeviceIdKey, deviceId);
                }
                return deviceId;
            }
            catch (Exception ex)
            {
                // Em caso de erro (ex: SecureStorage não disponível), gera um ID temporário
                return Guid.NewGuid().ToString();
            }
        }

        public static string GetDeviceName()
        {
            // Retorna um nome amigável para o dispositivo
            return $"{DeviceInfo.Current.Manufacturer} {DeviceInfo.Current.Model}";
        }
    }
}

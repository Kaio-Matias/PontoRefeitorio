// PontoRefeitorio/Services/AuthService.cs

using PontoRefeitorio.Helpers;
using PontoRefeitorio.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PontoRefeitorio.Services
{
    public class AuthService
    {
        private readonly ApiService _apiService;
        private static string _cachedToken;

        public AuthService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<bool> LoginAsync(string email, string senha)
        {
            try
            {
                // ==================================================================
                // CORREÇÃO APLICADA AQUI
                // ==================================================================
                // Chamando a versão correta e síncrona do helper
                var deviceIdentifier = DeviceInfoHelper.GetDeviceIdentifier();
                var deviceName = DeviceInfoHelper.GetDeviceName();

                var request = new LoginRequest
                {
                    Email = email,
                    Senha = senha,
                    DeviceIdentifier = deviceIdentifier,
                    NomeDispositivo = deviceName
                };

                var loginResponse = await _apiService.LoginAsync(request);

                if (!string.IsNullOrEmpty(loginResponse?.Token))
                {
                    await SetTokenAsync(loginResponse.Token);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro no login: {ex.Message}");
                return false;
            }
        }

        public Task<string> GetTokenAsync() => Task.FromResult(_cachedToken);

        public Task SetTokenAsync(string token)
        {
            _cachedToken = token;
            return Task.CompletedTask;
        }

        public void Logout()
        {
            _cachedToken = null;
        }
    }
}
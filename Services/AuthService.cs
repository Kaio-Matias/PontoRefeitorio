using Newtonsoft.Json;
using PontoRefeitorio.Models;
using System.Text;
using System.Net.Http.Json;

namespace PontoRefeitorio.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5114"; // IP para emulador Android

        public AuthService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginRequest);
                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                    {
                        await SecureStorage.SetAsync("auth_token", loginResponse.Token);
                        return loginResponse;
                    }
                }
            }
            catch (Exception ex)
            {
                return new LoginResponse { Message = $"Erro de conexão: {ex.Message}" };
            }
            return new LoginResponse { Message = "Utilizador ou senha inválidos." };
        }
    }
}